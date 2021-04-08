import * as React from 'react';
import Setting from '../interfaces/Setting';
import ArtifactType from '../interfaces/ArtifactType';
import KeyValueStringPair from '../interfaces/KeyValueStringPair';
import PricingTerm from '../interfaces/PricingTerm';
import { CUSTOM_REQUIRED_FIELD } from '../Constants';
import { useForm } from 'react-hook-form';

export const useArtifact = () => {

    const { handleSubmit, errors, setError, clearErrors, control } = useForm();
    const [artifactType, setArtifactType] = React.useState<ArtifactType | null>(null);
    const [name, setName] = React.useState<string | null>("");
    const [provider, setProvider] = React.useState<string | null>("");
    const [settingsList, setSettingsList] = React.useState<Setting[]>([{ name: "", value: "0" }]);
    const [settings, setSettings] = React.useState<object>({});
    const [settingTypes, setSettingTypes] = React.useState<{ [key: string]: string }>({});
    const [settingsMap, setSettingsMap] = React.useState<{ [key: string]: number[] }>({});
    const [awsSettingsList, setAwsSettingsList] = React.useState<KeyValueStringPair[]>([]);
    const [awsPricingTerm, setAwsPricingTerm] = React.useState<PricingTerm | null>(null);

    //const [settingsList, setSettingsList] = React.useState<Setting[]>(createSettingsListFromArtifact());
    const [price, setPrice] = React.useState(() => {
        let index = settingsList.findIndex(s => s.name === CUSTOM_REQUIRED_FIELD);
        if (index != -1) {
            let price = settingsList[index];
            settingsList.splice(index, 1);
            setSettingsList(settingsList);
            return price.value;
        }
        return 0;
    });
    const [artifactName, setArtifactName] = React.useState();

    //Validate Setting's Name


    const handleInputChange = (event: React.ChangeEvent<HTMLTextAreaElement | HTMLInputElement>, index: number) => {
        let { name, value } = event.target;
        name = name.split(".")[1];
        value = value.replace(/\s/g, '_').trim();
        if (name === 'name') {
            checkSettingName(value, index);
        }
        const list = [...settingsList];
        list[index][name] = value;
        setSettingsList(list);
    }

    const checkSettingName = (settingName: string, index: number) => {
        let mapList = { ...settingsMap };
        let key = searchIndexInObject(mapList, index);
        if (key != null) {
            deleteIndexFromObject(mapList, index, key);
        }
        if (!settingsMap.hasOwnProperty(settingName)) {
            mapList[settingName] = [index];
            setSettingsMap(mapList);
        }
        else {
            mapList[settingName].push(index);
            setSettingsMap(mapList);
        }
    }

    const searchIndexInObject = (object: { [key: string]: number[] }, index: number) => {
        for (let [key, array] of Object.entries(object)) {
            for (let i = 0; i < array.length; i++) {
                if (index === array[i]) {
                    return key;
                }
            }
        }
        return null;
    }

    //Delete the input index in settingsMap
    const deleteIndexFromObject = (object: { [key: string]: number[] }, index: number, key: string) => {
        if (key !== null) {
            object[key] = object[key].filter(number => number !== index);
            if (object[key].length === 0) {
                delete object[key];
            }
        }
    }

    //Set errors if the setting's name the user enters are repeat
    const setNameSettingsErrors = () => {
        for (let [, array] of Object.entries(settingsMap)) {
            if (array.length > 1) {
                for (let i = 0; i < array.length; i++) {
                    setError(`settings[${array[i]}].name`, {
                        type: "repeat",
                        message: "Los nombres no pueden repetirse"
                    });
                }
            }
            else if (array.length === 1) {
                clearErrors(`settings[${array[0]}].name`);
            }
        }
    }

    //Check if there are names repeated in settingsMap
    const areNamesRepeated = (index: number) => {
        let key = searchIndexInObject(settingsMap, index);
        if (key != null && (settingsMap[key].length > 1 || key === CUSTOM_REQUIRED_FIELD)) {
            return true;
        }
        return false;
    }

    const isFieldEmpty = (index: number, field: string, select: boolean) => {
        if (!select) {
            return (settingsList[index][field].trim() === "");
        }
        else {
            return (settingTypes[settingsList[index].name] === undefined || settingTypes[settingsList[index].name].trim() === "");
        }
    }

    const handleAddSetting = () => {
        setSettingsList([...settingsList, { name: "", value: "0" }]);
    }

    const handleDeleteSetting = (index: number) => {
        let listTypes = { ...settingTypes };
        delete listTypes[settingsList[index].name];
        setSettingTypes(listTypes);
        let listSettings = [...settingsList];
        listSettings.splice(index, 1);
        setSettingsList(listSettings);

        let mapList = { ...settingsMap };
        let key = searchIndexInObject(mapList, index);
        if (key != null) {
            deleteIndexFromObject(mapList, index, key);
            updateSettingsMap(mapList, index);

        }
        setSettingsMap(mapList);
    }

    const updateSettingsMap = (object: { [key: string]: number[] }, index: number) => {
        for (let [, array] of Object.entries(object)) {
            for (let i = 0; i < array.length; i++) {
                if (array[i] > index) {
                    array[i] = array[i] - 1;
                }
            }
        }
    }

    //Create and return the settings object for the create the artifact
    const createSettingsObject = () => {
        let settingsObject: { [key: string]: string } = {};

        for (let i = 0; i < settingsList.length; i++) {
            settingsObject[settingsList[i].name.trim().replace(/\s+/g, '')] = settingsList[i].value;
        }

        return settingsObject;
    }

    const handleTypeChange = (event: React.ChangeEvent<{ value: unknown }>, index: number) => {
        if (areNamesRepeated(index)) return;
        let auxSettingTypes = { ...settingTypes };
        auxSettingTypes[settingsList[index].name] = event.target.value as string;
        setSettingTypes(auxSettingTypes);
    }	

    return {
        handleInputChange: handleInputChange,
        handleAddSetting: handleAddSetting,
        handleDeleteSetting: handleDeleteSetting,
        settingsList: settingsList
	};
};