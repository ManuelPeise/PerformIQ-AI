import DashboardIcon from "@mui/icons-material/Dashboard";
import SettingsIcon from "@mui/icons-material/Settings";
import PeopleAltIcon from "@mui/icons-material/PeopleAlt";
import HealthAndSafetyIcon from "@mui/icons-material/HealthAndSafety";
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
      title: getResource("common.titleDashboard"),
      subtitle: getResource("common.subTitleDashboard"),
      icon: DashboardIcon,
      route: "/performiq-ai",
      showInNavigation: hasPermission(permissions, "dashboard"),
    },
    {
      title: getResource("common.titleHealthConnect"),
      subtitle: getResource("common.subTitleHealthConnectSubtitle"),
      icon: HealthAndSafetyIcon,
      route: "/performiq-ai/health-connect",
      showInNavigation: true, // hasPermission(permissions, "health_connect"),
    },
    {
      title: getResource("common.titleUserAdministration"),
      subtitle: getResource("common.subTitleUserAdministration"),
      icon: PeopleAltIcon,
      route: "/performiq-ai/user-administration",
      showInNavigation: hasPermission(permissions, "user_administration"),
    },
  ];
  return routes;
};
