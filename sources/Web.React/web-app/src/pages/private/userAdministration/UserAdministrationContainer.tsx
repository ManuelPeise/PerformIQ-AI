import React from "react";
import type { ILocalizationProps } from "../../../lib/localization/withLocalization";
import UserAdministration from "./UserAdministration";

interface IProps extends ILocalizationProps {}

const UserAdministrationContainer: React.FC<IProps> = (props) => {
  return <UserAdministration {...props} />;
};

export default UserAdministrationContainer;
