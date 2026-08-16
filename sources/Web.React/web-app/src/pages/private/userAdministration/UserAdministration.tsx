import React from "react";
import type { ILocalizationProps } from "../../../lib/localization/withLocalization";
import VerticalTabPage, {
  type IVerticalTabItem,
} from "../../../components/layouts/VerticalTabPage";
import DefaultPageContainer from "../../../components/layouts/DefaultPageContainer";
import { useAuthenticationContext } from "../../../hooks/useAuthenticationContext";
import type { IUserManagementInitializationProps } from "./UserAdministrationContainer";
import { utils } from "../../../lib/utils";
import type { IUserDataExportModel } from "./models/IUserDataExportModel";
import { type IHorizontalTabItem } from "../../../components/layouts/HorizontalTabPage";
import UserAdministrationTabPage from "./UserAdministrationTabPage";
import ManageAccountsIcon from "@mui/icons-material/ManageAccounts";
import AccessibilityIcon from "@mui/icons-material/Accessibility";
import UserDetails from "./UserDetails";
import UserAdministrationPlaceHolder from "./UserAdmisistrationPlaceHolder";
import FormWrapper from "../../../components/form/FormWrapper";
import UserPermissions from "./UserPermissions";
import { useSharedState } from "../../../hooks/useSharedState";

interface IUserAdministrationProps extends ILocalizationProps {}

const SelectedTabKeys = {
  userDetails: "userDetails",
  userPermissions: "userPermissions",
};

const UserAdministration: React.FC<
  IUserAdministrationProps & IUserManagementInitializationProps
> = (props) => {
  const { users, getResource, updateUserCallback } = props;
  const { currentUser, onLogoutUser } = useAuthenticationContext();
  const { data, isDirty, useModel, resetModel } =
    useSharedState<IUserDataExportModel>();

  const [selectedVerticalTab, setSelectedVerticalTab] =
    React.useState<IVerticalTabItem | null>(null);
  const [selectedHorizontalTabItem, setSelectedHorizontalTabItem] =
    React.useState<IHorizontalTabItem | null>(null);

  const horizontalTabItems = React.useMemo(() => {
    const items: IHorizontalTabItem[] = [
      {
        key: SelectedTabKeys.userDetails,
        title: getResource("common.titleUserDetails"),
        icon: ManageAccountsIcon,
        isReadonly:
          selectedHorizontalTabItem === null ||
          selectedHorizontalTabItem?.key === SelectedTabKeys.userDetails,
      },
      {
        key: SelectedTabKeys.userPermissions,
        title: getResource("common.titleUserPermissions"),
        icon: AccessibilityIcon,
        isReadonly:
          selectedVerticalTab === null ||
          selectedHorizontalTabItem?.key === SelectedTabKeys.userPermissions,
      },
    ];

    return items;
  }, [users, selectedHorizontalTabItem, selectedVerticalTab, getResource]);

  const verticalTabItems = React.useMemo(() => {
    return utils.mapToVerticalTabListItems<IUserDataExportModel>(
      (item) => item.userId,
      (item) =>
        currentUser && currentUser.userId === item.userId
          ? true
          : false || selectedVerticalTab?.key === item.userId
            ? true
            : false,
      (item) => item.userName,
      (item) => item.email,
      users.sort((a, b) => (a.userId > b.userId ? 1 : -1)),
    );
  }, [users, selectedVerticalTab, currentUser]);

  const handleSelectUser = React.useCallback(
    (item: IVerticalTabItem) => {
      const verticalTabItem =
        verticalTabItems.find((vItem) => vItem.key === item.key) ?? null;
      const user = users.find((user) => user.userId === item.key) ?? null;

      if (verticalTabItem != null && user != null) {
        console.log("Setting selected vertical tab and horizontal tab item");
        setSelectedVerticalTab(verticalTabItem);
        setSelectedHorizontalTabItem(horizontalTabItems[0]);
        useModel(user);
      }
    },
    [users, verticalTabItems, horizontalTabItems, useModel],
  );

  const handleSaveChanges = React.useCallback(async () => {
    if (isDirty && data != null && (await updateUserCallback(data))) {
      useModel({ ...data });
    }
  }, [isDirty, data, updateUserCallback, useModel]);

  React.useEffect(() => {
    if (users.length) {
      useModel(null);
    }
  }, [users]);

  return (
    <DefaultPageContainer
      onLogout={onLogoutUser}
      pageTitle="User Administration"
    >
      <VerticalTabPage
        getResource={getResource}
        items={verticalTabItems}
        selectedTab={selectedVerticalTab}
        onSelectTab={handleSelectUser}
        filterComponent={null}
      >
        <UserAdministrationTabPage
          items={horizontalTabItems}
          selectedItem={selectedHorizontalTabItem}
          onSelectTab={(item) => setSelectedHorizontalTabItem(item)}
          getResource={getResource}
        >
          <FormWrapper
            saveButtonText={getResource("common.labelSaveChanges")}
            cancelButtonText={getResource("common.labelCancel")}
            onCancel={resetModel}
            isModified={isDirty}
            onSave={handleSaveChanges}
          >
            {selectedVerticalTab == null ? (
              <UserAdministrationPlaceHolder getResource={getResource} />
            ) : selectedHorizontalTabItem?.key ===
              SelectedTabKeys.userDetails ? (
              <UserDetails getResource={getResource} />
            ) : (
              <UserPermissions getResource={getResource} />
            )}
          </FormWrapper>
        </UserAdministrationTabPage>
      </VerticalTabPage>
    </DefaultPageContainer>
  );
};

export default UserAdministration;
