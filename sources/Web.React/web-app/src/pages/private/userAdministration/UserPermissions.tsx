import React from "react";
import type { ILocalizationProps } from "../../../lib/localization/withLocalization";
import type { IUserDataExportModel } from "./models/IUserDataExportModel";
import StyledBox from "../../../components/styledComponents/boxes";
import { utils } from "../../../lib/utils";
import type { IPermission } from "../../../lib/types/auth/ICurrentUser";
import CollapsibleUserPermission from "./CollapsibleUserPermission";
import { useSharedState } from "../../../hooks/useSharedState";

interface IProps extends ILocalizationProps {}

const UserPermissions: React.FC<IProps> = (props) => {
  const { getResource } = props;

  const { data } = useSharedState<IUserDataExportModel>();

  const groupedPermissions = React.useMemo(() => {
    return utils.groupBy<IPermission, string>(
      data?.permissions ?? [],
      (permission) => permission.groupResourceKey,
    );
  }, [data?.permissions]);

  return (
    <StyledBox sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
      {Object.keys(groupedPermissions).map((groupResourceKey) => (
        <CollapsibleUserPermission
          key={groupResourceKey}
          title={getResource(groupResourceKey)}
          items={groupedPermissions[groupResourceKey]}
          getResource={getResource}
        />
      ))}
    </StyledBox>
  );
};

export default UserPermissions;
