import { PROVIDERS, CUSTOM_PROVIDER } from "../../Constants";
import Artifact from "../../interfaces/Artifact";
import KeyValueStringPair from "../../interfaces/KeyValueStringPair";

export function updateArtifactsSettings(
    artifact1: Artifact | null,
    artifact2: Artifact | null,
    setArtifact1Settings: Function,
    setArtifact2Settings: Function
) {
    if (artifact1 !== null && artifact1 !== undefined) {
        if (artifact1.artifactType?.name != CUSTOM_PROVIDER) {
            var relationalSettings: { [key: string]: string } = {};
            Object.entries(artifact1.relationalFields).map(([key, value]) => relationalSettings[key] = artifact1.settings[key]);
            setArtifact1Settings(relationalSettings);
        } else {
            setArtifact1Settings(artifact1.settings);
        }
    }
    else {
        setArtifact1Settings({});
    }
    if (artifact2 !== null && artifact2 !== undefined) {
        if (artifact2.artifactType?.name != CUSTOM_PROVIDER) {
            var relationalSettings: { [key: string]: string } = {};
            Object.entries(artifact2.relationalFields).map(([key, value]) => relationalSettings[key] = artifact2.settings[key]);
            setArtifact2Settings(relationalSettings);
        } else {
            setArtifact2Settings(artifact2.settings);
        }
    }
    else {
        setArtifact2Settings({});
    }
}

export function toRegularSentence(
    value: string
) {
    if (!value || value.length === 0) return "";

    const result = value.replace(/([A-Z])/g, ' $1');
    if (/[a-z]/.test(result.charAt(0))) {
        return `${result.charAt(0).toUpperCase()}${result.slice(1)}`;
    } else {
        return result;
    }
}

export function handleArtifactChange(
    event: React.ChangeEvent<{ name?: string | undefined; value: unknown }>,
    setSetting1: Function,
    setArtifact1: Function,
    setSetting2: Function,
    setArtifact2: Function,
    artifacts: Artifact[]
) {
    if (event.target.name === 'artifact1') {
        setSetting1(null);
        event.target.value === '' ?
            setArtifact1(null)
            :
            setArtifact1(artifacts.find(a => a.id === event.target.value) as Artifact);
    }
    else if (event.target.name === 'artifact2') {
        setSetting2(null);
        event.target.value === '' ?
            setArtifact2(null)
            :
            setArtifact2(artifacts.find(a => a.id === event.target.value) as Artifact);
    }
}

export function handleSettingChange(
    event: React.ChangeEvent<{ name?: string | undefined; value: unknown }>,
    artifact1Settings: { [key: string]: string },
    artifact2Settings: { [key: string]: string },
    setSetting1: Function,
    setSetting2: Function
) {
    if (event.target.name === 'setting1') {
        if (!event.target.value) {
            setSetting1(null);
            return;
        }
        let setting: KeyValueStringPair = { key: event.target.value as string, value: artifact1Settings[event.target.value as string] };
        setSetting1(setting);
    }
    else if (event.target.name === 'setting2') {
        if (!event.target.value) {
            setSetting2(null);
            return;
        }
        let setting: KeyValueStringPair = { key: event.target.value as string, value: artifact2Settings[event.target.value as string] };
        setSetting2(setting);
    }
}

export function differentSettingTypes(
    setting1: KeyValueStringPair | null,
    setting2: KeyValueStringPair | null,
    artifact1: Artifact | null,
    artifact2: Artifact | null
) {
    if (setting1 && setting2) {
        if (artifact1?.relationalFields[setting1.key] !== artifact2?.relationalFields[setting2.key]) {
            return true;
        }
    }
    return false;
}