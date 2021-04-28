import * as React from "react";
import Setting from '../interfaces/Setting';
import KeyValueStringPair from '../interfaces/KeyValueStringPair';
import PricingTerm from '../interfaces/PricingTerm';
import ArtifactType from '../interfaces/ArtifactType';
import { CUSTOM_REQUIRED_FIELD } from '../Constants';

export type ArtifactContext = {
    settingsList: Setting[]
    setSettingsList: (settingsList: Setting[]) => void,
    settingsMap: { [key: string]: number[] },
    setSettingsMap: (settingsMap: { [key: string]: number[] }) => void,
    settingTypes: { [key: string]: string },
    setSettingTypes: (settingTypes: { [key: string]: string }) => void,
    price: number,
    setPrice: (price: number) => void,
    settings: object,
    setSettings: (settings: object) => void,
    awsSettingsList: KeyValueStringPair[],
    setAwsSettingsList: (awsSettingsList: KeyValueStringPair[]) => void,
    awsPricingTerm: PricingTerm | null,
    setAwsPricingTerm: (awsPricingTerm: PricingTerm | null) => void,
    artifactType: ArtifactType | null,
    setArtifactType: (artifactType: ArtifactType | null) => void,
    name: string | null,
    setName: (name: string | null) => void,
    provider: string | null,
    setProvider: (provider: string | null) => void
}

export const ArtifactContext = React.createContext<ArtifactContext>({
    settingsList: [],
    setSettingsList: () => { },
    settingsMap: {},
    setSettingsMap: () => { },
    settingTypes: {},
    setSettingTypes: () => { },
    price: 0,
    setPrice: () => 0,
    settings: {},
    setSettings: () => { },
    awsSettingsList: [],
    setAwsSettingsList: () => { },
    awsPricingTerm: null,
    setAwsPricingTerm: () => { },
    artifactType: null,
    setArtifactType: () => { },
    name: "",
    setName: () => "",
    provider: "",
    setProvider: () => ""
});

export const useArtifactContext = () => React.useContext(ArtifactContext);

export const ArtifactContextProvider: React.FC<{}> = ({ children }) => {
    const [settingsList, setSettingsList] = React.useState<Setting[]>([{ name: "", value: "0" }]);
    const [settingsMap, setSettingsMap] = React.useState<{ [key: string]: number[] }>({});
    const [settingTypes, setSettingTypes] = React.useState<{ [key: string]: string }>({});
    const [settings, setSettings] = React.useState<object>({});
    const [price, setPrice] = React.useState<number>(() => {
        let index = settingsList.findIndex(s => s.name === CUSTOM_REQUIRED_FIELD);
        if (index != -1) {
            let price = settingsList[index];
            settingsList.splice(index, 1);
            setSettingsList(settingsList);
            return parseFloat(price.value);
        }
        return 0;
    });
    const [awsSettingsList, setAwsSettingsList] = React.useState<KeyValueStringPair[]>([]);
    const [awsPricingTerm, setAwsPricingTerm] = React.useState<PricingTerm | null>(null);
    const [artifactType, setArtifactType] = React.useState<ArtifactType | null>(null);
    const [name, setName] = React.useState<string | null>("");
    const [provider, setProvider] = React.useState<string | null>("");

    return <ArtifactContext.Provider value={{
        settingsList: settingsList,
        setSettingsList: setSettingsList,
        settingsMap: settingsMap,
        setSettingsMap: setSettingsMap,
        settingTypes: settingTypes,
        setSettingTypes: setSettingTypes,
        price: price,
        setPrice: setPrice,
        settings: settings,
        setSettings: setSettings,
        awsSettingsList: awsSettingsList,
        setAwsSettingsList: setAwsSettingsList,
        awsPricingTerm: awsPricingTerm,
        setAwsPricingTerm: setAwsPricingTerm,
        artifactType: artifactType,
        setArtifactType: setArtifactType,
        name: name,
        setName: setName,
        provider: provider,
        setProvider: setProvider
    }}>    
                { children }
    </ArtifactContext.Provider>
} 