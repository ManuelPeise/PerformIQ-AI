import React from "react";
import { utils } from "../../../lib/utils";
import type { IPermission } from "../../../lib/types/auth/ICurrentUser";
import type { ILocalizationProps } from "../../../lib/localization/withLocalization";
import StyledBox from "../../../components/styledComponents/boxes";
import Typography from "../../../components/styledComponents/typography";
import { IconButton } from "@mui/material";
import ArrowDropUpIcon from "@mui/icons-material/ArrowDropUp";
import ArrowDropDownIcon from "@mui/icons-material/ArrowDropDown";
import { StyledList } from "../../../components/styledComponents/listComponents";
import ListItemCheckboxGroup from "../../../components/list/ListItemCheckboxGroup";
import { useSharedState } from "../../../hooks/useSharedState";
import type { IUserDataExportModel } from "./models/IUserDataExportModel";

interface ICollapsibleUserPermissionProps extends ILocalizationProps {
  title: string;
  items: IPermission[];
}

const CollapsibleUserPermission: React.FC<ICollapsibleUserPermissionProps> = (
  props,
) => {
  const { title, items, getResource } = props;

  const { data, updateState } = useSharedState<IUserDataExportModel>();

  const [isOpen, setIsOpen] = React.useState(false);

  const toggleOpen = React.useCallback(() => {
    setIsOpen(!isOpen);
  }, [isOpen]);

  const handleUpdatePermission = React.useCallback(
    (
      index: number,
      permissionKey: string,
      key: keyof IPermission,
      value: boolean,
    ) => {
      if (data != null && (data.permissions ?? []).length > index) {
        const updatedPermissions = [...data.permissions];
        const indexOfPermission = updatedPermissions.findIndex(
          (permission) => permission.module === permissionKey,
        );

        if (indexOfPermission === -1) {
          return;
        }
        updatedPermissions[indexOfPermission] = {
          ...updatedPermissions[indexOfPermission],
          [key]: value,
        };

        updateState({
          ...data,
          permissions: updatedPermissions,
        });
      }
    },
    [updateState],
  );

  return (
    <StyledBox
      sx={{ height: "auto", display: "flex", flexDirection: "column", gap: 1 }}
    >
      <StyledBox
        sx={{
          height: "auto",
          borderColor: "divider",
          borderBottom: "1px solid",
          display: "flex",
          alignItems: "center",
          justifyContent: "space-between",
        }}
      >
        <Typography variant="h6" sx={{ display: "flex", alignItems: "center" }}>
          {title}
        </Typography>
        <IconButton onClick={toggleOpen} sx={{}}>
          {isOpen ? <ArrowDropUpIcon /> : <ArrowDropDownIcon />}
        </IconButton>
      </StyledBox>
      {isOpen && (
        <StyledBox sx={{ p: 0, m: 0 }}>
          <StyledList sx={{ width: "100%" }}>
            {items.map((permission, index) => (
              <ListItemCheckboxGroup
                key={index}
                groupName={permission.module}
                primary={utils.capitaliseFirstLetter(permission.module)}
                checkboxGroupProps={[
                  {
                    label: getResource("common.labelView"),
                    checked: permission.canView,
                    onChange: (checked) =>
                      handleUpdatePermission(
                        index,
                        permission.module,
                        "canView",
                        checked,
                      ),
                  },
                  {
                    label: getResource("common.labelCreate"),
                    checked: permission.canCreate,
                    onChange: (checked) =>
                      handleUpdatePermission(
                        index,
                        permission.module,
                        "canCreate",
                        checked,
                      ),
                  },
                  {
                    label: getResource("common.labelEdit"),
                    onChange: (checked) =>
                      handleUpdatePermission(
                        index,
                        permission.module,
                        "canEdit",
                        checked,
                      ),
                    checked: permission.canEdit,
                  },
                  {
                    label: getResource("common.labelDelete"),
                    checked: permission.canDelete,
                    onChange: (checked) =>
                      handleUpdatePermission(
                        index,
                        permission.module,
                        "canDelete",
                        checked,
                      ),
                  },
                ]}
              />
            ))}
          </StyledList>
        </StyledBox>
      )}
    </StyledBox>
  );
};

export default CollapsibleUserPermission;
