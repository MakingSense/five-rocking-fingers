import PricingDimension from './PricingDimension';

export default interface PricingTerm {
    sku: string;
    term: string;
    pricingDimensions: PricingDimension[];
    leaseContractLength: string;
    offeringClass: string;
    purchaseOption: string;
}