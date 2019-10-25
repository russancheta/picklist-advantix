import { Injectable } from '@angular/core';
import * as CryptoJS from 'crypto-js';

@Injectable({
  providedIn: 'root'
})
export class ReportService {

  constructor() { }

  setEncryptedData(details: string) {
    const key = CryptoJS.enc.Utf8.parse('iloveyouaivy0714');
    const iv = CryptoJS.enc.Utf8.parse('iloveyouaivy0714');
    const keySettings = {
      keySize: 128 / 8,
      iv: iv,
      mode: CryptoJS.mode.CBC,
      padding: CryptoJS.pad.Pkcs7
    };
    const encryptedString = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(details), key, keySettings);
    return encryptedString;
  }
}
