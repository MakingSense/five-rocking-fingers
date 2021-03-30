import PricingTerm from '../../interfaces/PricingTerm';
import PricingDimension from '../../interfaces/PricingDimension';
import { AMAZON_EC2, COMPUTE_INSTACE, STORAGE, STORAGE_SNAPSHOT, SYSTEM_OPERATION, PROVISIONED_THROUGHPUT, DATA_TRANSFER_1, DATA_TRANSFER_2, DATA_TRANSFER_3, AMAZON_S3 } from '../../Constants';

export const useSettingsCreator = () => {

	const createAmazonEc2Settings = (awsPricingDimensionList: PricingTerm[]): object => {
		let amazonEc2Settings = {};

		awsPricingDimensionList.forEach(awsPricingDimension => {

			let settings = {};

			switch (awsPricingDimension.product) {
				case COMPUTE_INSTACE:
					settings = createPricingTermObject('product0', awsPricingDimension);
					break;
				case STORAGE:
					settings = createPricingTermObject('product1', awsPricingDimension);
					break;
				case STORAGE_SNAPSHOT:
					settings = createPricingTermObject('product2', awsPricingDimension);
					break;
				case SYSTEM_OPERATION:
					settings = createPricingTermObject('product3', awsPricingDimension);
					break;
				case PROVISIONED_THROUGHPUT:
					settings = createPricingTermObject('product4', awsPricingDimension);
					break;
				case DATA_TRANSFER_1:
					settings = createPricingTermObject('product5-1', awsPricingDimension);
					break;
				case DATA_TRANSFER_2:
					settings = createPricingTermObject('product5-2', awsPricingDimension);
					break;
				case DATA_TRANSFER_3:
					settings = createPricingTermObject('product5-3', awsPricingDimension);
					break;
			}

			Object.assign(amazonEc2Settings, amazonEc2Settings, settings);
		});

		return amazonEc2Settings;
	};

	const createAmazonS3Settings = (awsPricingDimensionList: PricingTerm[]): object => {
		let amazonS3Settings = {};

		awsPricingDimensionList.forEach((awsPricingDimension, index) => {

			let settings = createPricingTermObject(`product${index}`, awsPricingDimension);			

			Object.assign(amazonS3Settings, amazonS3Settings, settings);
		});

		return amazonS3Settings;
	};

	const createPricingTermObject = (productName: string, awsPricingDimension: PricingTerm): { [product: string]: { sku: string, term: string, leaseContractLength: string, offeringClass: string, purchaseOption: string, pricingDimensions: { [key: string]: PricingDimension } } } => {
		let pricingTermObject: { [product: string]: { sku: string, term: string, leaseContractLength: string, offeringClass: string, purchaseOption: string, pricingDimensions: { [key: string]: PricingDimension } } } = {};

		pricingTermObject[productName] = {
			sku: awsPricingDimension.sku,
			term: awsPricingDimension.term,
			leaseContractLength: awsPricingDimension.leaseContractLength,
			offeringClass: awsPricingDimension.offeringClass,
			purchaseOption: awsPricingDimension.purchaseOption,
			pricingDimensions: {}
		};

		let pricingDimensionObject: { [key: string]: PricingDimension } = {};

		awsPricingDimension.pricingDimensions.forEach((pricingDimension, i) => {
			pricingDimensionObject[`range${i}`] = pricingDimension;
		});

		pricingTermObject[productName].pricingDimensions = pricingDimensionObject;

		return pricingTermObject;
	}

	const createPricingTermSettings = (serviceCode: string, awsPricingDimensionList: PricingTerm[]): object => {
		let pricingTermSetting = {};

		switch (serviceCode) {
			case AMAZON_EC2:
				pricingTermSetting = createAmazonEc2Settings(awsPricingDimensionList);
				break;
			case AMAZON_S3:
				pricingTermSetting = createAmazonS3Settings(awsPricingDimensionList);
				break;
		}

		return pricingTermSetting;
	};

	return {
		createPricingTermSettings: createPricingTermSettings
	};
};