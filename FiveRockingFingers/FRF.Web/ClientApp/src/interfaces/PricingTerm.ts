import PricingDimension from './PricingDimension';

export default interface PricingTerm {
    product: string;
    sku: string;
    term: string;
    pricingDimensions: PricingDimension[];
    leaseContractLength: string;
    offeringClass: string;
    purchaseOption: string;
}