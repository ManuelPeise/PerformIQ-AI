import React from "react";
import type { ILocalizationProps } from "../../lib/localization/withLocalization";
import type { PropsWithChildren } from "react";
import StyledBox from "../styledComponents/boxes";
import type { OverridableComponent } from "@mui/material/OverridableComponent";
import type { SvgIconTypeMap } from "@mui/material/SvgIcon";
import {
  StyledList,
  StyledListItemButton,
  StyledListItemIcon,
  StyledListItemText,
} from "../styledComponents/listComponents";

export interface IHorizontalTabItem {
  key: string | number;
  title: string;
  icon?: OverridableComponent<SvgIconTypeMap<{}, "svg">> & {
    muiName: string;
  };
  isReadonly?: boolean;
}

interface IProps extends PropsWithChildren<ILocalizationProps> {
  items: IHorizontalTabItem[];
  selectCallback?: (item: IHorizontalTabItem) => void;
}

const HorizontalTabPage: React.FC<IProps> = (props) => {
  const { items, selectCallback, children } = props;

  return (
    <StyledBox sx={{ display: "flex", flexDirection: "column" }}>
      <StyledList
        sx={{
          display: "flex",
          flexDirection: "row",
          p: 0,
          m: 0,
          borderBottom: "1px solid",
          borderColor: "divider",
        }}
      >
        {items.map((item) => (
          <StyledListItemButton
            key={item.key}
            disabled={item.isReadonly}
            onClick={() => selectCallback && selectCallback(item)}
            sx={{
              display: "flex",
              flexDirection: "row",
              alignItems: "baseline",
              p: 2,
              ":not(:last-child)": {
                borderRight: "1px solid",
                borderColor: "divider",
              },
            }}
          >
            {item.icon && (
              <StyledListItemIcon>
                <item.icon />
              </StyledListItemIcon>
            )}
            <StyledListItemText primary={item.title} />
          </StyledListItemButton>
        ))}
      </StyledList>
      <StyledBox sx={{ p: 2, flex: 1 }}>{children}</StyledBox>
    </StyledBox>
  );
};

export default HorizontalTabPage;
