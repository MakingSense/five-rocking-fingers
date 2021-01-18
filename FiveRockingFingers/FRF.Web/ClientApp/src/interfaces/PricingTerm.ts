import PricingDimension from './PricingDimension';

export default interface PricingTerm {
    sku: string;
    term: string;
    pricingDimension: PricingDimension;
    pricingDetail: PricingDimension;
    leaseContractLength: string;
    offeringClass: string;
    purchaseOption: string;
}