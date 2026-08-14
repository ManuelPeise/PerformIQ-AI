import React from "react";
import i18n from "./i18n";

export interface ILocalizationProps {
  getResource: (key: string) => string;
}

export function withLocalization<P extends ILocalizationProps>(
  namespaces: string[],
  Component: React.FC<P>,
): React.FC<Omit<P, keyof ILocalizationProps>> {
  const LocalizedComponent: React.FC<Omit<P, keyof ILocalizationProps>> = (
    props,
  ) => {
    const getResource = (key: string): string => {
      const [namespace, ...keyParts] = key.split(".");

      if (!namespaces.includes(namespace)) {
        console.warn(
          `Translation namespace "${namespace}" is not declared. ` +
            `Available namespaces: ${namespaces.join(", ")}`,
        );

        return key;
      }

      const translationKey = keyParts.join(".");

      return i18n.t(translationKey, {
        ns: namespace,
      });
    };

    return <Component {...(props as P)} getResource={getResource} />;
  };

  LocalizedComponent.displayName = `withLocalization(${
    Component.displayName || Component.name || "Component"
  })`;

  return LocalizedComponent;
}
