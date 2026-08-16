import React, { type PropsWithChildren } from "react";
import StyledBox from "../styledComponents/boxes";
import StyledPaper from "../styledComponents/paper";
import { ListItemButton, ListItemText } from "@mui/material";
import type { ILocalizationProps } from "../../lib/localization/withLocalization";

export type IVerticalTabItem = {
  key: string | number;
  title: string;
  subtitle?: string;
  isReadonly?: boolean;
};

interface IVerticalTabPageProps extends PropsWithChildren, ILocalizationProps {
  selectedTab?: IVerticalTabItem | null;
  items: IVerticalTabItem[];
  filterComponent?: React.ComponentType<{}> | null;
  onSelectTab: (item: IVerticalTabItem) => void;
}

const VerticalTabPage: React.FC<IVerticalTabPageProps> = (props) => {
  const {
    items,
    children,
    filterComponent,
    selectedTab: selectedTabProp,
    onSelectTab,
  } = props;

  const FilterComponent = filterComponent ?? null;

  return (
    <StyledBox
      sx={{
        display: "flex",
        flexDirection: "column",
        height: "100%",
        minHeight: 0,
      }}
    >
      {/* Oberer Bereich */}

      {FilterComponent && (
        <StyledBox
          sx={{
            width: "100%",
            maxHeight: 120,
            boxSizing: "border-box",
            overflow: "auto",
          }}
        >
          <StyledPaper sx={{ width: "100%" }}>
            <FilterComponent />
          </StyledPaper>
        </StyledBox>
      )}

      {/* Unterer Bereich */}
      <StyledBox
        sx={{
          flex: 1,
          minHeight: 0,
          display: "grid",
          gridTemplateColumns: "300px minmax(0, 1fr)",
          gap: 2,
        }}
      >
        {/* Navigation */}
        <StyledPaper
          sx={{
            overflow: "auto",
          }}
        >
          {items.map((item, index) => (
            <ListItemButton
              key={index}
              selected={selectedTabProp === item}
              disabled={item.isReadonly}
              onClick={() => onSelectTab(item)}
            >
              <ListItemText primary={item.title} secondary={item.subtitle} />
            </ListItemButton>
          ))}
        </StyledPaper>

        {/* Content */}
        <StyledPaper
          sx={{
            minWidth: 0,
            minHeight: 0,
            overflow: "auto",
          }}
        >
          {children}
        </StyledPaper>
      </StyledBox>
    </StyledBox>
  );
};

export default VerticalTabPage;
