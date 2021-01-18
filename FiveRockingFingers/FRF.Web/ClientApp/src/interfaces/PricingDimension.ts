export default interface PricingDimension {
    unit: string;
    endRange: string;
    description: string;
    rateCode: string;
    beginRange: string;
    currency: string;
    pricePerUnit: number;
}