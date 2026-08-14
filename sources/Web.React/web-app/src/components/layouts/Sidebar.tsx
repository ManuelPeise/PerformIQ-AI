import React from "react";
import type { ILocalizationProps } from "../../lib/localization/withLocalization";
import { useCurrentUser } from "../../hooks/useCurrentUser";
import { getNavigarionRoutes } from "../../lib/navigation/navigationRoutes";
import StyledBox from "../styledComponents/boxes";
import ArrowCircleLeftIcon from "@mui/icons-material/ArrowCircleLeft";
import ArrowCircleRightIcon from "@mui/icons-material/ArrowCircleRight";
import AccountCircleIcon from "@mui/icons-material/AccountCircle";
import {
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
} from "@mui/material";

import { useNavigate } from "react-router-dom";

const sidebarMinWidth = 80;
const sidebarMaxWidth = 300;
const iconWith = 40;

interface ISidebarProps extends ILocalizationProps {
  isExpanded?: boolean;
  toggleSidebar?: () => void;
}

const Sidebar: React.FC<ISidebarProps> = (props) => {
  const { isExpanded = true, toggleSidebar = () => {} } = props;
  const { getResource } = props;
  const { currentUser } = useCurrentUser();
  const navigate = useNavigate();

  const [currentSelectedRoute, setCurrentSelectedRoute] =
    React.useState<string>("/performiq-ai");

  const navigationItems = React.useMemo(() => {
    return getNavigarionRoutes(getResource, currentUser?.permissions ?? []);
  }, [currentUser, getResource]);

  const sideBarWidth = React.useMemo(() => {
    return isExpanded ? sidebarMaxWidth : sidebarMinWidth;
  }, [isExpanded]);

  const handleNavigation = React.useCallback(
    (route: string) => {
      setCurrentSelectedRoute(route);
      navigate(route);
    },
    [navigate],
  );

  return (
    <StyledBox
      sx={{
        width: sideBarWidth,
        backgroundColor: "background.paper",
        margin: 0,
        padding: 0,
        height: "100%",
      }}
    >
      <List>
        <ListItem
          divider
          sx={{
            display: "flex",
            justifyContent: "center",
            width: sideBarWidth,
          }}
        >
          {isExpanded && (
            <ListItemText
              primary={getResource("common.labelAppShortName")}
              secondary={getResource("common.labelAppName")}
            />
          )}
          <ListItemIcon>
            {isExpanded ? (
              <ArrowCircleLeftIcon
                sx={{
                  width: iconWith,
                  height: iconWith,
                  opacity: 0.5,
                  "&:hover": { opacity: 1 },
                }}
                onClick={() => toggleSidebar()}
              />
            ) : (
              <ArrowCircleRightIcon
                sx={{
                  width: iconWith,
                  height: iconWith,
                  opacity: 0.5,
                  "&:hover": { opacity: 1 },
                }}
                onClick={() => toggleSidebar()}
              />
            )}
          </ListItemIcon>
        </ListItem>
        <ListItem
          divider
          sx={{
            display: "flex",
            justifyContent: "center",
            width: sideBarWidth,
            backgroundColor: "background.paper",
          }}
        >
          <ListItemIcon>
            <AccountCircleIcon
              sx={{
                width: iconWith,
                height: iconWith,
                opacity: isExpanded ? 1 : 0.5,
              }}
            />
          </ListItemIcon>
          {isExpanded && (
            <ListItemText
              primary={currentUser?.userName}
              secondary={currentUser?.email}
              sx={{
                paddingLeft: 2,
              }}
            />
          )}
        </ListItem>
        {navigationItems.map((item, key) => (
          <ListItemButton
            key={key}
            divider
            disabled={!isExpanded || currentSelectedRoute === item.route}
            onClick={() => handleNavigation(item.route)}
            sx={{
              display: "flex",
              justifyContent: "center",
              width: sideBarWidth,
            }}
          >
            <ListItemIcon>
              <item.icon sx={{ width: iconWith, height: iconWith }} />
            </ListItemIcon>
            {isExpanded && (
              <ListItemText
                primary={item.title}
                secondary={item.subtitle}
                sx={{
                  paddingLeft: 2,
                }}
              />
            )}
          </ListItemButton>
        ))}
      </List>
    </StyledBox>
  );
};

export default Sidebar;
