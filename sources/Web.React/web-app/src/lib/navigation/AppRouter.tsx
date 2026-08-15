import React from "react";
import UserAdministrationContainer from "../../pages/private/userAdministration/UserAdministration";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import LandingPage from "../../pages/public/LandingPage";
import RegistrationPage from "../../pages/public/RegistrationPage";
import LoginPage from "../../pages/public/LoginPage";
import UnauthorizedLayout from "../../components/layouts/UnauthorizedLayout";
import AuthorizedLayout from "../../components/layouts/AuthorizedLayout";
import PageNotFound from "../../pages/PageNotFound";
import type { ILocalizationProps } from "../localization/withLocalization";
import { getResource } from "../localization/i18n";
import Dashboard from "../../pages/private/dashboard/Dashboard";
import HealthConnectContainer from "../../pages/private/healthConnect/HealthConnectContainer";

const AppRouter: React.FC = () => {
  const localeProps: ILocalizationProps = {
    getResource: getResource,
  };

  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<UnauthorizedLayout />}>
          <Route path="/" element={<LandingPage {...localeProps} />} />
          <Route
            path="/auth/register"
            element={<RegistrationPage {...localeProps} />}
          />
          <Route path="/auth/login" element={<LoginPage {...localeProps} />} />
        </Route>
        <Route
          path="/performiq-ai"
          element={<AuthorizedLayout {...localeProps} />}
        >
          <Route
            path="/performiq-ai"
            element={<Dashboard {...localeProps} />}
          />
          <Route
            path="/performiq-ai/user-administration"
            element={<UserAdministrationContainer {...localeProps} />}
          />
          <Route
            path="/performiq-ai/health-connect"
            element={<HealthConnectContainer {...localeProps} />}
          />
        </Route>
        <Route path="*" element={<PageNotFound />} />
      </Routes>
    </BrowserRouter>
  );
};

export default AppRouter;
