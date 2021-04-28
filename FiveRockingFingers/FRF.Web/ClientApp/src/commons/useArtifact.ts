import * as React from 'react';
import Setting from '../interfaces/Setting';
import ArtifactType from '../interfaces/ArtifactType';
import KeyValueStringPair from '../interfaces/KeyValueStringPair';
import PricingTerm from '../interfaces/PricingTerm';
import { useForm } from 'react-hook-form';
import { SETTINGTYPES, CUSTOM_REQUIRED_FIELD } from '../Constants';
import { ArtifactContext } from "./ArtifactContext";
import { useContext } from "react";

export const useArtifact = () => {

    const {
        settingsList,
        setSettingsList,
        settingsMap,
        setSettingsMap,
        settingTypes,
        setSettingTypes,
        price,
        setPrice,
        settings,
        setSettings,
        awsSettingsList,
        setAwsSettingsList,
        awsPricingTerm,
        setAwsPricingTerm,
        artifactType,
        setArtifactType,
        name,
        setName,
        provider,
        setProvider
    } = useContext(ArtifactContext);

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

    const isPriceValid = () => {
        if (price >= 0) return true
        return false
    }

    const handleChangePrice = (event: React.ChangeEvent<HTMLTextAreaElement | HTMLInputElement>) => {
        setPrice(parseInt(event.target.value, 10));
    }

    const isValidNumber = (index: number): boolean => {
        return !isNaN(Number(settingsList[index].value)) ? Math.sign(Number(settingsList[index].value)) >= 0 ? true : false : false;
    }

    const isNumberSameType = (index: number) => {
        const numberType = settingTypes[settingsList[index].name];
        if (numberType === undefined) return true;
        const settingValue = parseFloat(settingsList[index].value);
        if (numberType === SETTINGTYPES[1]) {
            return Boolean(settingValue % 1 === 0);
        }
        else if (numberType === SETTINGTYPES[0]) {
            return Boolean(settingValue % 1 !== 0 || settingValue % 1 === 0);
        }
    }

    const resetStateOnClose = () => {
        setName("");
        setProvider("");
        setSettingsList([{ name: "", value: "" }]);
        setSettingsMap({});
        setSettingTypes({});
        setPrice(0);
        setAwsSettingsList([]);
        setAwsPricingTerm(null);
        setArtifactType(null);
    }

    return {
        resetStateOnClose: resetStateOnClose,
        handleInputChange: handleInputChange,
        handleTypeChange: handleTypeChange,
        handleAddSetting: handleAddSetting,
        handleDeleteSetting: handleDeleteSetting,
        isPriceValid: isPriceValid,
        settingsList: settingsList,
        setSettingsList: setSettingsList,
        settingsMap: settingsMap,
        setSettingsMap: setSettingsMap,
        price: price,
        setPrice: setPrice,
        handleChangePrice: handleChangePrice,
        areNamesRepeated: areNamesRepeated,
        isFieldEmpty: isFieldEmpty,
        isValidNumber: isValidNumber,
        isNumberSameType: isNumberSameType,
        settingTypes: settingTypes,
        setSettingTypes: setSettingTypes,
        createSettingsObject: createSettingsObject,
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
	};
};