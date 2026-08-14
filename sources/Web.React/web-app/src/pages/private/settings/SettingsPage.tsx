import React from "react";
import type { ILocalizationProps } from "../../../lib/localization/withLocalization";

interface IProps extends ILocalizationProps {}

const SettingsPage: React.FC<IProps> = () => {
  return <div>Settings Page</div>;
};

export default SettingsPage;
