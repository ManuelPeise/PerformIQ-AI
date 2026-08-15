import React from "react";
import type { ILocalizationProps } from "../../../lib/localization/withLocalization";
import HealthConnect from "./HealthConnect";

interface IProps extends ILocalizationProps {}

const HealthConnectContainer: React.FC<IProps> = (props) => {
  return <HealthConnect {...props} />;
};

export default HealthConnectContainer;
