import PricingTerm from '../../interfaces/PricingTerm';
import { AMAZON_EC2, COMPUTE_INSTACE, AMAZON_S3 } from '../../Constants';

export const usePricingDimensionsValidator = () => {

	const areValidAmazonEc2PricingDimensions = (awsPricingDimensionList: PricingTerm[]): boolean => {
		if (awsPricingDimensionList.length === 0) return false;
		for (let i = 0; i < awsPricingDimensionList.length; i++) {
			let awsPricingDimension = awsPricingDimensionList[i];
			if (awsPricingDimension.product === COMPUTE_INSTACE)
				return true;
		}
		return false;
	};

	const areValidAmazonS3PricingDimensions = (awsPricingDimensionList: PricingTerm[]): boolean => {
		if (awsPricingDimensionList.length === 0) return false;
		return true;
	};

	const areValidPricingDimensions = (serviceCode: string, awsPricingDimensionList: PricingTerm[]): boolean => {
		let validPricingDimensions = false;
		switch (serviceCode) {
			case AMAZON_EC2:
				validPricingDimensions = areValidAmazonEc2PricingDimensions(awsPricingDimensionList);
				break;
			case AMAZON_S3:
				validPricingDimensions = areValidAmazonS3PricingDimensions(awsPricingDimensionList);
				break;
		}

		return validPricingDimensions;
	};

	return {
		areValidPricingDimensions: areValidPricingDimensions
	};
};