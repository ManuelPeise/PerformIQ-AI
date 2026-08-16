import React from "react";
import type { ILocalizationProps } from "../../../lib/localization/withLocalization";
import { StyledList } from "../../../components/styledComponents/listComponents";
import ListItemTextField from "../../../components/list/ListItemTextField";
import ListItemSwitch from "../../../components/list/ListItemSwitch";
import ListItemCheckboxGroup from "../../../components/list/ListItemCheckboxGroup";
import ListItemCheckbox from "../../../components/list/ListItemCheckbox";
import { useSharedState } from "../../../hooks/useSharedState";
import type { IUserDataExportModel } from "./models/IUserDataExportModel";

interface IProps extends ILocalizationProps {}

const UserDetails: React.FC<IProps> = (props) => {
  const { getResource } = props;
  const { data, updateState } = useSharedState<IUserDataExportModel>();

  const handleUserRolesChanged = React.useCallback(
    (role: string, checked: boolean) => {
      let intermediateRoles =
        checked && !data?.roles?.includes(role)
          ? [...(data?.roles ?? []), role]
          : (data?.roles?.filter((r) => r !== role) ?? []);

      if (!intermediateRoles.length) {
        intermediateRoles.push("User");
      }

      if (data != null) {
        updateState({
          roles: intermediateRoles,
        });
      }
    },
    [data?.roles],
  );

  const handleMarkedAsDeletedChanged = React.useCallback(
    (checked: boolean) => {
      if (checked && data != null) {
        updateState({
          isMarkedAsDeleted: checked,
          isActive: false,
        });

        return;
      }

      if (data != null) {
        updateState({
          isMarkedAsDeleted: checked,
        });
      }
    },
    [data?.isMarkedAsDeleted],
  );

  return (
    <StyledList>
      <ListItemTextField
        primary={getResource("common.labelFirstName")}
        secondary={getResource("common.labelFirstNameDescription")}
        textFieldProps={{
          value: data?.firstName ?? "",
          onChange:
            data != null
              ? (value) =>
                  updateState({
                    firstName: value,
                  })
              : () => {},

          disabled: true,
        }}
      />
      <ListItemTextField
        primary={getResource("common.labelLastName")}
        secondary={getResource("common.labelLastNameDescription")}
        textFieldProps={{
          value: data?.lastName ?? "",
          onChange:
            data != null
              ? (value) =>
                  updateState({
                    lastName: value,
                  })
              : () => {},
          disabled: true,
        }}
      />
      <ListItemTextField
        primary={getResource("common.labelUserName")}
        secondary={getResource("common.labelUserNameDescription")}
        textFieldProps={{
          value: data?.userName ?? "",
          onChange:
            data != null
              ? (value) =>
                  updateState({
                    userName: value,
                  })
              : () => {},
          disabled: true,
        }}
      />
      <ListItemTextField
        primary={getResource("common.labelEmailAddress")}
        secondary={getResource("common.labelEmailAddressDescription")}
        textFieldProps={{
          value: data?.email ?? "",
          onChange:
            data != null
              ? (value) =>
                  updateState({
                    email: value,
                  })
              : () => {},
          disabled: true,
        }}
      />
      <ListItemSwitch
        primary={getResource("common.labelIsActive")}
        secondary={getResource("common.labelIsActiveDescription")}
        switchProps={{
          checked: data?.isActive ?? false,
          onChange:
            data != null
              ? (checked) =>
                  updateState({
                    isActive: checked,
                  })
              : () => {},
          disabled: false,
        }}
      />
      <ListItemCheckboxGroup
        primary={getResource("common.labelRoles")}
        secondary={getResource("common.labelRolesDescription")}
        groupName="roles"
        checkboxGroupProps={[
          {
            label: getResource("common.labelIsUser"),
            checked: data?.roles?.includes("User") ?? false,
            onChange: (checked) => handleUserRolesChanged("User", checked),
            disabled: false,
          },
          {
            label: getResource("common.labelIsAdmin"),
            checked: data?.roles?.includes("Admin") ?? false,
            onChange: (checked) => handleUserRolesChanged("Admin", checked),
            disabled: false,
          },
        ]}
      />
      <ListItemCheckbox
        primary={getResource("common.labelIsMarkedAsDeleted")}
        secondary={getResource("common.labelIsMarkedAsDeletedDescription")}
        checkboxProps={{
          label: getResource("common.labelIsMarkedAsDeleted"),
          checked: data?.isMarkedAsDeleted ?? false,
          onChange: (checked) => handleMarkedAsDeletedChanged(checked),
          disabled: false,
        }}
      />
    </StyledList>
  );
};

export default UserDetails;
