import React, { type PropsWithChildren } from "react";
import type { ILocalizationProps } from "../../../lib/localization/withLocalization";
import HorizontalTabPage, {
  type IHorizontalTabItem,
} from "../../../components/layouts/HorizontalTabPage";

interface IProps extends PropsWithChildren<ILocalizationProps> {
  items: IHorizontalTabItem[];
  selectedItem?: IHorizontalTabItem | null;
  onSelectTab?: (item: IHorizontalTabItem) => void;
}

const UserAdministrationTabPage: React.FC<IProps> = (props) => {
  const { items, getResource, children, onSelectTab } = props;

  return (
    <HorizontalTabPage
      getResource={getResource}
      items={items}
      selectCallback={onSelectTab}
    >
      {children}
    </HorizontalTabPage>
  );
};

export default UserAdministrationTabPage;
