import { createRoot } from "react-dom/client";
import App from "./App.tsx";
import "./styles/app.css";
import "./lib/localization/i18n.ts";
import { CssBaseline, ThemeProvider } from "@mui/material";
import { createAppTheme } from "./styles/theme";
import AuthenticationContextProvider from "./components/context/AuthContextProvider.tsx";
import CurrentUserProvider from "./components/context/CurrentUserContext.tsx";

const theme = createAppTheme("dark");

createRoot(document.getElementById("root")!).render(
  <ThemeProvider theme={theme}>
    <CssBaseline />
    <CurrentUserProvider>
      <AuthenticationContextProvider>
        <App />
      </AuthenticationContextProvider>
    </CurrentUserProvider>
  </ThemeProvider>,
);
