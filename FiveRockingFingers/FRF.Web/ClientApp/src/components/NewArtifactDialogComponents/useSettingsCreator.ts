import PricingTerm from '../../interfaces/PricingTerm';
import PricingDimension from '../../interfaces/PricingDimension';

export const useSettingsCreator = () => {

	const createAmazonEc2Settings = (awsPricingDimensionList: PricingTerm[]): object => {
		let amazonEc2Settings = {};

		awsPricingDimensionList.forEach(awsPricingDimension => {

			let settings = {};

			switch (awsPricingDimension.product) {
				case 'Compute Instance':
					settings = createPricingTermObject('product0', awsPricingDimension);
					break;
				case 'Storage':
					settings = createPricingTermObject('product1', awsPricingDimension);
					break;
				case 'Storage Snapshot':
					settings = createPricingTermObject('product2', awsPricingDimension);
					break;
				case 'System Operation':
					settings = createPricingTermObject('product3', awsPricingDimension);
					break;
				case 'Provisioned Throughput':
					settings = createPricingTermObject('product4', awsPricingDimension);
					break;
				case 'Data Transfer':
					settings = createPricingTermObject('product5-1', awsPricingDimension);
					break;
			}

			Object.assign(amazonEc2Settings, amazonEc2Settings, settings);
		});

		return amazonEc2Settings;
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
			case 'AmazonEC2':
				pricingTermSetting = createAmazonEc2Settings(awsPricingDimensionList);
				break;
		}

		return pricingTermSetting;
	};

	return {
		createPricingTermSettings: createPricingTermSettings
	};
};