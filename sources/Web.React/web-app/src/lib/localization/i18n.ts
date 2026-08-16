// src/i18n/config.ts

import i18n from "i18next";
import { initReactI18next } from "react-i18next";

import en from "./en/common.json";
import de from "./de/common.json";

i18n.use(initReactI18next).init({
  resources: {
    en: {
      common: en,
    },
    de: {
      common: de,
    },
  },
  ns: ["common"],
  lng: "en",
  fallbackLng: "en",
  defaultNS: "common",
  interpolation: {
    escapeValue: false,
  },
});

export const getResource = (key: string): string => {
  const [namespace, ...translationKeys] = key.split(".");

  return i18n.t(translationKeys.join("."), {
    ns: namespace,
  });
};

export default i18n;
