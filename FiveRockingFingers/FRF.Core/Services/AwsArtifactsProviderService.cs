using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FRF.Core.Base;
using Microsoft.Extensions.Options;
using Amazon.Pricing;
using Amazon.Pricing.Model;
using FRF.Core.Models;
using FRF.Core.Response;
using System;

namespace FRF.Core.Services
{
    public class AwsArtifactsProviderService : IArtifactsProviderService
    {
        private const string OffersCodeIndex = "offers.*.offerCode";
        private readonly AwsPricing _awsPricingOptions;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IAmazonPricing _pricingClient;

        public AwsArtifactsProviderService(IOptions<AwsPricing> awsApiString,
            IHttpClientFactory httpClientFactory, IAmazonPricing pricingClient)
        {
            _httpClientFactory = httpClientFactory;
            _awsPricingOptions = awsApiString.Value;
            _pricingClient = pricingClient;
        }

        /// <summary>
        /// Get all the artifacts names with the service code from aws pricing api.
        /// </summary>
        /// <returns>List of KeyValuePair</returns>
        public async Task<ServiceResponse<List<KeyValuePair<string, string>>>> GetNamesAsync()
        {
            var artifactsNames = new Dictionary<string, string>();
            var httpClient = _httpClientFactory.CreateClient();

            var response = await httpClient.GetAsync(_awsPricingOptions.ApiUrl);
            if (!response.IsSuccessStatusCode)
            {
                return new ServiceResponse<List<KeyValuePair<string, string>>>(
                    new Error(ErrorCodes.AmazonApiError, "There was an error connecting to the Amazon API"));
            }

            var pricingList = await response.Content.ReadAsStringAsync();
            var awsArtifactsNames = JObject
                .Parse(pricingList)
                .SelectTokens(OffersCodeIndex);

            foreach (var artifactName in awsArtifactsNames)
            {
                var serviceCode = artifactName.ToString();
                var name = ExtractName(artifactName.ToString());
                artifactsNames.Add(serviceCode, name);
            }

            return new ServiceResponse<List<KeyValuePair<string, string>>>(artifactsNames.ToList());
        }

        public async Task<ServiceResponse<List<ProviderArtifactSetting>>> GetAttributesAsync(string serviceCode)
        {
            var attributes = new List<ProviderArtifactSetting>();
            var response = await _pricingClient.DescribeServicesAsync(new DescribeServicesRequest
            {
                FormatVersion = "aws_v1",
                ServiceCode = serviceCode
            });

            if(response == null)
            {
                return new ServiceResponse<List<ProviderArtifactSetting>>(attributes);
            }

            foreach (var service in response.Services)
            {
                foreach (var attributeName in service.AttributeNames)
                {
                    var attributeKeyValue = new KeyValuePair<string, string>(attributeName, ExtractName(attributeName));
                    var attribute = new ProviderArtifactSetting();
                    attribute.Name = attributeKeyValue;
                    attribute.Values = await GetAttributeValue(attributeName, serviceCode);
                    attributes.Add(attribute);
                }
            }

            return new ServiceResponse<List<ProviderArtifactSetting>>(attributes);
        }

        private async Task<List<string>> GetAttributeValue(string attributeName, string serviceCode)
        {
            var attributeValues = new List<string>();

            var response = await _pricingClient.GetAttributeValuesAsync(new GetAttributeValuesRequest
            {
                AttributeName = attributeName,
                ServiceCode = serviceCode
            });

            if(response == null)
            {
                return attributeValues;
            }

            foreach (var attributeValue in response.AttributeValues)
            {
                attributeValues.Add(attributeValue.Value);
            }

            return attributeValues;
        }

        public async Task<ServiceResponse<List<PricingTerm>>> GetProductsAsync(
            List<KeyValuePair<string, string>> settings, string serviceCode)
        {
            switch (serviceCode)
            {
                case AwsS3Descriptions.Service:
                    return await GetS3ProductsAsync(settings, false);
                case AwsEc2Descriptions.ServiceValue:
                    return await GetEc2ProductsAsync(settings);
                default:
                    return await GetDefaultProductsAsync(settings, serviceCode);
            }
        }

        private async Task<ServiceResponse<List<PricingTerm>>> GetDefaultProductsAsync(List<KeyValuePair<string, string>> settings, string serviceCode)
        {
            var filters = new List<Filter>();

            var pricingDetailsList = new List<PricingTerm>();

            foreach (var setting in settings)
            {
                var filter = new Filter
                {
                    Field = setting.Key,
                    Type = "TERM_MATCH",
                    Value = setting.Value
                };

                filters.Add(filter);
            }

            var response = await _pricingClient.GetProductsAsync(new GetProductsRequest
            {
                Filters = filters,
                FormatVersion = "aws_v1",
                ServiceCode = serviceCode
            });

            if(response == null)
            {
                return new ServiceResponse<List<PricingTerm>>(pricingDetailsList);
            }
            AddProductToPricingDetails(response, pricingDetailsList);
            return new ServiceResponse<List<PricingTerm>>(pricingDetailsList);
        }

        private async Task<ServiceResponse<List<PricingTerm>>> GetS3ProductsAsync(
            List<KeyValuePair<string, string>> settings, bool isAutomaticMonitoring)
        {
            var pricingDetailsList = new List<PricingTerm>();
            var storageFilters = new List<Filter>();
            var writeRequestFilters = new List<Filter>();
            var retrieveRequestFilters = new List<Filter>();
            var locationFilter = new Filter {Field = AwsS3Descriptions.Location, Type = "TERM_MATCH", Value = ""};
            var storageClassFilter = new Filter {Field = AwsS3Descriptions.StorageClass, Type = "TERM_MATCH", Value = ""};
            var volumeTypeFilter = new Filter {Field = AwsS3Descriptions.VolumeType, Type = "TERM_MATCH", Value = ""};
            var writeRequestGroupValue = AwsS3Descriptions.WriteFrequentGroup;
            var retrieveRequestGroupValue = AwsS3Descriptions.RetrieveFrequentGroup;

            foreach (var (key, value) in settings)
            {
                var storageFilter = new Filter {Field = key, Type = "TERM_MATCH", Value = value};

                storageFilters.Add(storageFilter);
                switch (key)
                {
                    case AwsS3Descriptions.Location:
                        locationFilter.Value = value;
                        break;
                    case AwsS3Descriptions.StorageClass:
                        storageClassFilter.Value = value;
                        break;
                    case AwsS3Descriptions.VolumeType:
                        volumeTypeFilter.Value = value;
                        break;
                }
            }

            var storagePrice = await _pricingClient.GetProductsAsync(new GetProductsRequest
            {
                Filters = storageFilters,
                FormatVersion = "aws_v1",
                ServiceCode = AwsS3Descriptions.Service
            });

            if (storagePrice == null || !storagePrice.PriceList.Any()) return new ServiceResponse<List<PricingTerm>>(pricingDetailsList);

            AddProductToPricingDetails(storagePrice, pricingDetailsList);

            //Check if the product is Standard - Infrequent Access.
            if (volumeTypeFilter.Value.Equals(
                AwsS3Descriptions.StandardInfrequentAccessProduct, StringComparison.InvariantCultureIgnoreCase))
            {
                writeRequestGroupValue = AwsS3Descriptions.WriteInfrequentGroup;
                retrieveRequestGroupValue = AwsS3Descriptions.RetrieveInfrequentGroup;
            }
            //Check if the product is One Zone - Infrequent Access.
            else if (volumeTypeFilter.Value.Equals(AwsS3Descriptions.InfrequentAccessProduct,
                StringComparison.InvariantCultureIgnoreCase))
            {
                writeRequestGroupValue = AwsS3Descriptions.WriteOneZoneInfrequentGroup;
                retrieveRequestGroupValue = AwsS3Descriptions.RetrieveOneZoneInfrequentGroup;
            }

            writeRequestFilters.Add(locationFilter);
            writeRequestFilters.Add(new Filter {Field = "group", Type = "TERM_MATCH", Value = writeRequestGroupValue});

            var writeRequestPrice = await _pricingClient.GetProductsAsync(new GetProductsRequest
            {
                Filters = writeRequestFilters,
                FormatVersion = "aws_v1",
                ServiceCode = AwsS3Descriptions.Service
            });

            if (writeRequestPrice == null || !storagePrice.PriceList.Any()) return new ServiceResponse<List<PricingTerm>>(pricingDetailsList);

            AddProductToPricingDetails(writeRequestPrice, pricingDetailsList);

            retrieveRequestFilters.Add(locationFilter);
            retrieveRequestFilters.Add(new Filter
                {Field = "group", Type = "TERM_MATCH", Value = retrieveRequestGroupValue});

            var retrieveRequestPrice = await _pricingClient.GetProductsAsync(new GetProductsRequest
            {
                Filters = retrieveRequestFilters,
                FormatVersion = "aws_v1",
                ServiceCode = AwsS3Descriptions.Service
            });

            if (retrieveRequestPrice == null || !storagePrice.PriceList.Any()) return new ServiceResponse<List<PricingTerm>>(pricingDetailsList);

            AddProductToPricingDetails(retrieveRequestPrice, pricingDetailsList);

            //Check if the product is Intelligent-Tiering.
            if (storageClassFilter.Value.Equals(AwsS3Descriptions.IntelligentTieringProduct, StringComparison.InvariantCultureIgnoreCase))
                pricingDetailsList =
                    await GetS3IntelligentTieringDetailListAsync(locationFilter, pricingDetailsList, isAutomaticMonitoring);

            return new ServiceResponse<List<PricingTerm>>(pricingDetailsList);
        }

        private async Task<List<PricingTerm>> GetS3IntelligentTieringDetailListAsync(Filter locationFilter,
            List<PricingTerm> pricingDetailsList, bool isAutomaticMonitoring)
        {
            var infrequentAccessFilters = new List<Filter>();

            infrequentAccessFilters.Add(locationFilter);
            infrequentAccessFilters.Add(new Filter
                { Field = AwsS3Descriptions.VolumeType, Type = "TERM_MATCH", Value = AwsS3Descriptions.IntelligentInfrequentAccessProduct });
            var infrequentAccessPrice = await _pricingClient.GetProductsAsync(new GetProductsRequest
            {
                Filters = infrequentAccessFilters,
                FormatVersion = "aws_v1",
                ServiceCode = AwsS3Descriptions.Service
            });

            AddProductToPricingDetails(infrequentAccessPrice, pricingDetailsList);

            if (!isAutomaticMonitoring) return pricingDetailsList;

            var monitoringFilters = new List<Filter>
            {
                locationFilter,
                new Filter
                    {Field = "feeCode", Type = "TERM_MATCH", Value = AwsS3Descriptions.AutomationObjectCountFee}
            };

            var automaticMonitoringPrice = await _pricingClient.GetProductsAsync(new GetProductsRequest
            {
                Filters = monitoringFilters,
                FormatVersion = "aws_v1",
                ServiceCode = AwsS3Descriptions.Service
            });

            AddProductToPricingDetails(automaticMonitoringPrice, pricingDetailsList);

            return pricingDetailsList;
        }

        private async Task<ServiceResponse<List<PricingTerm>>> GetEc2ProductsAsync(List<KeyValuePair<string, string>> settings)
        {
            //Filters get from settings
            var locationFilter = new Filter { Field = AwsEc2Descriptions.Location, Type = "TERM_MATCH", Value = "" };
            var operatingSystemFilter = new Filter { Field = AwsEc2Descriptions.OperatingSystem, Type = "TERM_MATCH", Value = "" };
            var preInstalledSwFilter = new Filter { Field = AwsEc2Descriptions.PreInstalledSw, Type = "TERM_MATCH", Value = "" };
            var instanceTypeFilter = new Filter { Field = AwsEc2Descriptions.InstanceType, Type = "TERM_MATCH", Value = "" };
            var vcpuFilter = new Filter { Field = AwsEc2Descriptions.Vcpu, Type = "TERM_MATCH", Value = "" };
            var memoryFilter = new Filter { Field = AwsEc2Descriptions.Memory, Type = "TERM_MATCH", Value = "" };
            var networkPerformanceFilter = new Filter { Field = AwsEc2Descriptions.NetworkPerformance, Type = "TERM_MATCH", Value = "" };
            var storageFilter = new Filter { Field = AwsEc2Descriptions.Storage, Type = "TERM_MATCH", Value = "" };
            var volumeApiNameFilter = new Filter { Field = AwsEc2Descriptions.VolumeApiName, Type = "TERM_MATCH", Value = "" };
            
            var transferTypeFilter1 = new Filter { Field = AwsEc2Descriptions.TransferType, Type = "TERM_MATCH", Value = "" };
            var fromLocationFilter1 = new Filter { Field = AwsEc2Descriptions.FromLocation, Type = "TERM_MATCH", Value = "" };
            var toLocationFilter1 = new Filter { Field = AwsEc2Descriptions.ToLocation, Type = "TERM_MATCH", Value = "" };

            var transferTypeFilter2 = new Filter { Field = AwsEc2Descriptions.TransferType, Type = "TERM_MATCH", Value = "" };
            var fromLocationFilter2 = new Filter { Field = AwsEc2Descriptions.FromLocation, Type = "TERM_MATCH", Value = "" };
            var toLocationFilter2 = new Filter { Field = AwsEc2Descriptions.ToLocation, Type = "TERM_MATCH", Value = "" };

            var transferTypeFilter3 = new Filter { Field = AwsEc2Descriptions.TransferType, Type = "TERM_MATCH", Value = "" };
            var fromLocationFilter3 = new Filter { Field = AwsEc2Descriptions.FromLocation, Type = "TERM_MATCH", Value = "" };
            var toLocationFilter3 = new Filter { Field = AwsEc2Descriptions.ToLocation, Type = "TERM_MATCH", Value = "" };

            var termAttributes = new TermAttributes();

            //Filters that don't came in the settings
            var locationTypeFilter = new Filter { Field = AwsEc2Descriptions.LocationType, Type = "TERM_MATCH", Value = AwsEc2Descriptions.LocationTypeValue };
            var toLocationTypeFilter = new Filter { Field = AwsEc2Descriptions.ToLocationType, Type = "TERM_MATCH", Value = AwsEc2Descriptions.LocationTypeValue };
            var fromLocationTypeFilter = new Filter { Field = AwsEc2Descriptions.FromLocationType, Type = "TERM_MATCH", Value = AwsEc2Descriptions.LocationTypeValue };
            var serviceCodeFilter = new Filter { Field = AwsEc2Descriptions.ServiceCode, Type = "TERM_MATCH", Value = AwsEc2Descriptions.ServiceValue };
            var productFamilyComputeInstanceFilter = new Filter { Field = AwsEc2Descriptions.ProductFamily, Type = "TERM_MATCH", Value = AwsEc2Descriptions.ProductFamilyComputeInstanceValue };
            var productFamilyEbsStorageFilter = new Filter { Field = AwsEc2Descriptions.ProductFamily, Type = "TERM_MATCH", Value = AwsEc2Descriptions.ProductFamilyEbsStorageValue };
            var productFamilyEbsSnapshotFilter = new Filter { Field = AwsEc2Descriptions.ProductFamily, Type = "TERM_MATCH", Value = AwsEc2Descriptions.ProductFamilyEbsSnapshotsValue };
            var productFamilyIopsFilter = new Filter { Field = AwsEc2Descriptions.ProductFamily, Type = "TERM_MATCH", Value = AwsEc2Descriptions.ProductFamilyEbsIopsValue };
            var productFamilyThroughputFilter = new Filter { Field = AwsEc2Descriptions.ProductFamily, Type = "TERM_MATCH", Value = AwsEc2Descriptions.ProductFamilyEbsThroughputValue };
            var productFamilyDataTransferFilter = new Filter { Field = AwsEc2Descriptions.ProductFamily, Type = "TERM_MATCH", Value = AwsEc2Descriptions.ProductFamilyDataTransferValue };
            
            var pricingDetailsList = new List<PricingTerm>();

            foreach (var (key, value) in settings)
            {
                switch (key)
                {
                    case AwsEc2Descriptions.Location:
                        locationFilter.Value = value;
                        break;
                    case AwsEc2Descriptions.OperatingSystem:
                        operatingSystemFilter.Value = value;
                        break;
                    case AwsEc2Descriptions.PreInstalledSw:
                        preInstalledSwFilter.Value = value;
                        break;
                    case AwsEc2Descriptions.InstanceType:
                        instanceTypeFilter.Value = value;
                        break;
                    case AwsEc2Descriptions.Vcpu:
                        vcpuFilter.Value = value;
                        break;
                    case AwsEc2Descriptions.Memory:
                        memoryFilter.Value = value;
                        break;
                    case AwsEc2Descriptions.NetworkPerformance:
                        networkPerformanceFilter.Value = value;
                        break;
                    case AwsEc2Descriptions.Storage:
                        storageFilter.Value = value;
                        break;
                    case AwsEc2Descriptions.VolumeApiName:
                        volumeApiNameFilter.Value = value;
                        break;
                    case AwsEc2Descriptions.TransferType1:
                        transferTypeFilter1.Value = value;
                        break;
                    case AwsEc2Descriptions.FromLocation1:
                        fromLocationFilter1.Value = value;
                        break;
                    case AwsEc2Descriptions.ToLocation1:
                        toLocationFilter1.Value = value;
                        break;
                    case AwsEc2Descriptions.TransferType2:
                        transferTypeFilter2.Value = value;
                        break;
                    case AwsEc2Descriptions.FromLocation2:
                        fromLocationFilter2.Value = value;
                        break;
                    case AwsEc2Descriptions.ToLocation2:
                        toLocationFilter2.Value = value;
                        break;
                    case AwsEc2Descriptions.TransferType3:
                        transferTypeFilter3.Value = value;
                        break;
                    case AwsEc2Descriptions.FromLocation3:
                        fromLocationFilter3.Value = value;
                        break;
                    case AwsEc2Descriptions.ToLocation3:
                        toLocationFilter3.Value = value;
                        break;
                    case AwsEc2Descriptions.TermType:
                        termAttributes.TermType = value;
                        break;
                    case AwsEc2Descriptions.PurchaseOption:
                        termAttributes.PurchaseOption = value;
                        break;
                    case AwsEc2Descriptions.OfferingClass:
                        termAttributes.OfferingClass = value;
                        break;
                    case AwsEc2Descriptions.LeaseContractLength:
                        termAttributes.LeaseContractLength = value;
                        break;
                }
            }

            var computeInstanceFilters = new List<Filter>
            {
                productFamilyComputeInstanceFilter,
                locationFilter,
                operatingSystemFilter,
                preInstalledSwFilter,
                instanceTypeFilter,
                vcpuFilter,
                memoryFilter,
                networkPerformanceFilter,
                storageFilter,
                locationTypeFilter,
                serviceCodeFilter
            };

            var ebsStorageFilters = new List<Filter>
            {
                productFamilyEbsStorageFilter,
                locationFilter,
                volumeApiNameFilter,
                locationTypeFilter,
                serviceCodeFilter
            };

            var ebsSnapshotFilters = new List<Filter>
            {
                productFamilyEbsSnapshotFilter,
                locationFilter,
                locationTypeFilter,
                serviceCodeFilter
            };

            var iopsFilters = new List<Filter>
            {
                productFamilyIopsFilter,
                locationFilter,
                locationTypeFilter,
                volumeApiNameFilter,
                serviceCodeFilter
            };

            var throughputFilters = new List<Filter>
            {
                productFamilyThroughputFilter,
                locationFilter,
                locationTypeFilter,
                volumeApiNameFilter,
                serviceCodeFilter
            };

            var dataTransferFilters1 = new List<Filter>
            {
                productFamilyDataTransferFilter,
                transferTypeFilter1,
                fromLocationFilter1,
                toLocationFilter1,
                toLocationTypeFilter,
                fromLocationTypeFilter,
                serviceCodeFilter
            };

            var dataTransferFilters2 = new List<Filter>
            {
                productFamilyDataTransferFilter,
                transferTypeFilter2,
                fromLocationFilter2,
                toLocationFilter2,
                toLocationTypeFilter,
                fromLocationTypeFilter,
                serviceCodeFilter
            };

            var dataTransferFilters3 = new List<Filter>
            {
                productFamilyDataTransferFilter,
                transferTypeFilter3,
                fromLocationFilter3,
                toLocationFilter3,
                toLocationTypeFilter,
                fromLocationTypeFilter,
                serviceCodeFilter
            };

            await CompletePricingListOfAmazonEc2(computeInstanceFilters, ebsStorageFilters, ebsSnapshotFilters,
                iopsFilters, throughputFilters, dataTransferFilters1, dataTransferFilters2, dataTransferFilters3,
                pricingDetailsList, termAttributes);

            return new ServiceResponse<List<PricingTerm>>(pricingDetailsList);
        }

        public async Task CompletePricingListOfAmazonEc2(List<Filter> computeInstanceFilters,
            List<Filter> ebsStorageFilters, List<Filter> ebsSnapshotFilters, List<Filter> iopsFilters,
            List<Filter> throughputFilters, List<Filter> dataTransferFilters1, List<Filter> dataTransferFilters2,
            List<Filter> dataTransferFilters3, List<PricingTerm> pricingDetailsList, TermAttributes termAttributes)
        {
            var isComputeInstanceValid = await AddProductsToPricingList(AwsEc2Descriptions.ServiceValue,
                computeInstanceFilters, pricingDetailsList, AwsEc2Descriptions.ProductFamilyComputeInstanceValue,
                termAttributes);
            if(!isComputeInstanceValid)
            {
                return;
            }
            await AddProductsToPricingList(AwsEc2Descriptions.ServiceValue, ebsStorageFilters, pricingDetailsList, AwsEc2Descriptions.ProductFamilyEbsStorageValue);
            await AddProductsToPricingList(AwsEc2Descriptions.ServiceValue, ebsSnapshotFilters, pricingDetailsList, AwsEc2Descriptions.ProductFamilyEbsSnapshotsValue);
            await AddProductsToPricingList(AwsEc2Descriptions.ServiceValue, iopsFilters, pricingDetailsList, AwsEc2Descriptions.ProductFamilyEbsIopsValue);
            await AddProductsToPricingList(AwsEc2Descriptions.ServiceValue, throughputFilters, pricingDetailsList, AwsEc2Descriptions.ProductFamilyEbsThroughputValue);
            await AddProductsToPricingList(AwsEc2Descriptions.ServiceValue, dataTransferFilters1, pricingDetailsList, AwsEc2Descriptions.ProductFamilyDataTransferValue);
            await AddProductsToPricingList(AwsEc2Descriptions.ServiceValue, dataTransferFilters2, pricingDetailsList, AwsEc2Descriptions.ProductFamilyDataTransferValue);
            await AddProductsToPricingList(AwsEc2Descriptions.ServiceValue, dataTransferFilters3, pricingDetailsList, AwsEc2Descriptions.ProductFamilyDataTransferValue);
        }

        public async Task<bool> AddProductsToPricingList(string serviceCode, List<Filter> filters, List<PricingTerm> pricingDetailsList,
            string product, TermAttributes termAttributes = null)
        {
            var isAnyFilterInvalid = filters.Any(f => string.IsNullOrEmpty(f.Value));

            if (!isAnyFilterInvalid)
            {
                if(product.Equals(AwsEc2Descriptions.ProductFamilyComputeInstanceValue))
                {
                    await SearchComputeInstaceAsync(serviceCode, filters, pricingDetailsList, product, termAttributes);
                }
                else
                {
                    await SearchProductsAsync(serviceCode, filters, pricingDetailsList, product);
                }

                return true;
            }

            return false;
        }

        private async Task SearchProductsAsync(string serviceCode, List<Filter> filters,
            List<PricingTerm> pricingDetailsList, string product)
        {
            var response = await _pricingClient.GetProductsAsync(new GetProductsRequest
            {
                Filters = filters,
                FormatVersion = "aws_v1",
                ServiceCode = serviceCode
            });

            if(response != null)
            {
                AddProductToPricingDetails(response, pricingDetailsList, product);
            }            
        }

        private async Task SearchComputeInstaceAsync(string serviceCode, List<Filter> computeInstanceFilters,
            List<PricingTerm> pricingDetailsList, string product, TermAttributes termAttributes)
        {
            var response = await _pricingClient.GetProductsAsync(new GetProductsRequest
            {
                Filters = computeInstanceFilters,
                FormatVersion = "aws_v1",
                ServiceCode = serviceCode
            });

            if (response != null)
            {
                AddProductToPricingDetails(response, pricingDetailsList, product, termAttributes);
            }
        }

        private void AddProductToPricingDetails(GetProductsResponse response,
            List<PricingTerm> pricingDetailsList, string product = "", TermAttributes termAttributes = null)
        {
            foreach (var price in response.PriceList)
            {
                if (!price.StartsWith("{") || !price.EndsWith("}")) continue;

                var priceJson = JObject.Parse(price);
                if (priceJson == null) continue;

                var sku = priceJson.SelectToken("product.sku").Value<string>();
                var terms = priceJson.SelectToken("terms").ToObject<JObject>();
                if (terms == null) continue;

                foreach (var term in terms.Properties())
                {
                    var termName = term.Name;
                    var termProperties = term.Value.ToObject<JObject>();
                    if (termProperties == null)
                    {
                        continue;
                    }

                    foreach (var termOption in termProperties)
                    {
                        AddProduct(termOption, pricingDetailsList, termName, sku, product, termAttributes);
                    }
                }
            }
        }

        public void AddProduct(KeyValuePair<string, JToken> termOption, List<PricingTerm> pricingDetailsList,
            string termName, string sku, string product, TermAttributes termAttributes)
        {
            if(product.Equals(AwsEc2Descriptions.ProductFamilyComputeInstanceValue))
            {
                if (termAttributes.TermType.Equals(AwsEc2Descriptions.OnDemandTermType) && termName.Equals(termAttributes.TermType))
                {
                    var pricingTerm = CreatePricingTerm(sku, termName, termOption, product);
                    if (pricingTerm.PricingDimensions[0].PricePerUnit == 0 || pricingDetailsList.Any(pd => pd.Product.Equals(AwsEc2Descriptions.ProductFamilyComputeInstanceValue)))
                    {
                        return;
                    }
                    pricingDetailsList.Add(pricingTerm);
                }

                var termOptionProperties = termOption.Value;
                var purchaseOption = (string)termOptionProperties.SelectToken("termAttributes.PurchaseOption");
                var leaseContractLength = (string)termOptionProperties.SelectToken("termAttributes.LeaseContractLength");
                var offeringClass = (string)termOptionProperties.SelectToken("termAttributes.OfferingClass");

                if (purchaseOption != null &&
                    leaseContractLength != null &&
                    offeringClass != null &&
                    termName.Equals(termAttributes.TermType) &&
                    purchaseOption.Equals(termAttributes.PurchaseOption) &&
                    leaseContractLength.Equals(termAttributes.LeaseContractLength) &&
                    offeringClass.Equals(termAttributes.OfferingClass))
                {
                    var pricingTerm = CreatePricingTerm(sku, termName, termOption, product);
                    pricingDetailsList.Add(pricingTerm);
                }
            }
            else
            {
                var pricingTerm = CreatePricingTerm(sku, termName, termOption, product);
                pricingDetailsList.Add(pricingTerm);
            }
        }

        private PricingTerm CreatePricingTerm(string sku, string termName, KeyValuePair<string, JToken> termOption, string product)
        {
            var termOptionProperties = termOption.Value;
            var leaseContractLength = (string)termOptionProperties.SelectToken("termAttributes.LeaseContractLength");
            var offeringClass = (string)termOptionProperties.SelectToken("termAttributes.OfferingClass");
            var purchaseOption = (string)termOptionProperties.SelectToken("termAttributes.PurchaseOption");
            var termPriceDimensions = termOptionProperties.SelectToken("priceDimensions").ToObject<JObject>().Properties().ToList();
            var pricingDimensions = new List<PricingDimension>();

            foreach (var termPriceDimension in termPriceDimensions)
            {
                var pricingDimension = ExtractPricingDimension(termPriceDimension);
                pricingDimensions.Add(pricingDimension);
            }

            var pricingTerm = new PricingTerm()
            {
                Product = product,
                Sku = sku,
                Term = termName,
                PricingDimensions = pricingDimensions,
                LeaseContractLength = leaseContractLength,
                OfferingClass = offeringClass,
                PurchaseOption = purchaseOption
            };

            return pricingTerm;
        }

        private PricingDimension ExtractPricingDimension(JProperty termPriceDimension)
        {
            var termValues = termPriceDimension.Value;
            
            if (termValues.SelectToken("endRange") != null && termValues.SelectToken("endRange").Value<string>().Equals("Inf",StringComparison.InvariantCultureIgnoreCase)) termValues["endRange"] = -1;

            var pricePerUnitJObject = termValues.SelectToken("pricePerUnit").ToObject<JObject>().Properties().ElementAt(0);
            var currency = pricePerUnitJObject.Name;
            var pricePerUnit = (decimal) pricePerUnitJObject.Value;

            termValues.SelectToken("pricePerUnit").Replace(pricePerUnit);

            var pricingDimension = termValues.ToObject<PricingDimension>();
            pricingDimension.Currency = currency;
            return pricingDimension;
        }

        private static string ExtractName(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return string.Empty;

            var splittedString = Regex.Replace(
                Regex.Replace(str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"),
                @"(\p{Ll})(\P{Ll})", "$1 $2");
            var firstUpperAndSplittedString = splittedString.ToCharArray();
            firstUpperAndSplittedString[0] = char.ToUpper(firstUpperAndSplittedString[0]);

            return new string(firstUpperAndSplittedString);
        }
    }
}
