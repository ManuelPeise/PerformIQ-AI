import DashboardIcon from "@mui/icons-material/Dashboard";
import SettingsIcon from "@mui/icons-material/Settings";
import type { OverridableComponent } from "@mui/material/OverridableComponent";
import type { SvgIconTypeMap } from "@mui/material/SvgIcon";
import type { IPermission } from "../types/auth/ICurrentUser";

export type NavigationRoute = {
  icon: OverridableComponent<SvgIconTypeMap<{}, "svg">> & {
    muiName: string;
  };
  title: string;
  subtitle: string;
  route: string;
  showInNavigation: boolean;
};

const hasPermission = (permissions: IPermission[], module: string): boolean => {
  return permissions.some(
    (permission) => permission.module === module && permission.canView,
  );
};

export const getNavigarionRoutes = (
  getResource: (key: string) => string,
  permissions: IPermission[],
): NavigationRoute[] => {
  const routes: NavigationRoute[] = [
    {
      title: getResource("common.dashboard"),
      subtitle: getResource("common.dashboardSubtitle"),
      icon: DashboardIcon,
      route: "/performiq-ai",
      showInNavigation: hasPermission(permissions, "dashboard"),
    },
    {
      title: getResource("common.settings"),
      subtitle: getResource("common.settingsSubtitle"),
      icon: SettingsIcon,
      route: "/performiq-ai/settings",
      showInNavigation: hasPermission(permissions, "settings"),
    },
  ];
  return routes;
};
