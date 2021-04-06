import * as React from 'react';

export function extractErrorCode(errorData: string): number {
    let text: RegExp = new RegExp(/\b(^|\s)([0-9]+)($|:){1}\B/g);
    return parseInt(text.exec(errorData)![0]);
}