import React, { type PropsWithChildren } from "react";
import StyledBox from "../styledComponents/boxes";
import StyledPaper from "../styledComponents/paper";
import { ListItemButton, ListItemText, Paper } from "@mui/material";
import type { ILocalizationProps } from "../../lib/localization/withLocalization";

type IVerticalTabItem = {
  title: string;
  subtitle?: string;
  component: React.ComponentType<any>;
  isReadonly?: boolean;
};

interface IVerticalTabPageProps extends PropsWithChildren, ILocalizationProps {
  items: IVerticalTabItem[];
}

const VerticalTabPage: React.FC<IVerticalTabPageProps> = (props) => {
  const { items, children } = props;

  const [selectedTab, setSelectedTab] = React.useState<IVerticalTabItem | null>(
    null,
  );

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
      {children && (
        <StyledBox
          sx={{
            width: "100%",
            maxHeight: 120,
            boxSizing: "border-box",
            p: 2,
          }}
        >
          <StyledPaper sx={{ width: "100%" }}>{children}</StyledPaper>
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
          px: 0,
          pb: 0,
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
              selected={selectedTab === item}
              onClick={() => setSelectedTab(item)}
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
            p: 2,
          }}
        >
          {selectedTab && <selectedTab.component />}
        </StyledPaper>
      </StyledBox>
    </StyledBox>
  );
};

export default VerticalTabPage;
