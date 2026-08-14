import { createTheme, type PaletteMode } from "@mui/material/styles";

export const createAppTheme = (mode: PaletteMode) =>
  createTheme({
    palette: {
      mode,

      primary: {
        main: "#6366F1",
        light: "#818CF8",
        dark: "#4F46E5",
        contrastText: "#FFFFFF",
      },

      secondary: {
        main: "#8B5CF6",
        light: "#A78BFA",
        dark: "#7C3AED",
        contrastText: "#FFFFFF",
      },

      ...(mode === "light"
        ? {
            background: {
              default: "#F8FAFC",
              paper: "#FFFFFF",
            },

            text: {
              primary: "#18181B",
              secondary: "#71717A",
            },

            divider: "#E4E4E7",
          }
        : {
            background: {
              default: "#09090B",
              paper: "#18181B",
            },

            text: {
              primary: "#FAFAFA",
              secondary: "#A1A1AA",
            },

            divider: "#27272A",
          }),
      success: {
        main: "#22C55E",
      },

      warning: {
        main: "#F59E0B",
      },

      error: {
        main: "#EF4444",
      },

      info: {
        main: "#3B82F6",
      },
    },

    typography: {
      fontFamily: [
        "Inter",
        "system-ui",
        "-apple-system",
        "BlinkMacSystemFont",
        '"Segoe UI"',
        "sans-serif",
      ].join(","),

      h1: {
        fontSize: "2.5rem",
        fontWeight: 700,
        letterSpacing: "-0.025em",
      },

      h2: {
        fontSize: "2rem",
        fontWeight: 700,
        letterSpacing: "-0.02em",
      },

      h3: {
        fontSize: "1.5rem",
        fontWeight: 600,
      },

      h4: {
        fontSize: "1.25rem",
        fontWeight: 600,
      },

      body1: {
        fontSize: "1rem",
        lineHeight: 1.6,
      },

      body2: {
        fontSize: "0.875rem",
        lineHeight: 1.5,
      },

      button: {
        fontWeight: 600,
        textTransform: "none",
      },
    },

    shape: {
      borderRadius: 10,
    },

    spacing: 8,

    components: {
      MuiButton: {
        defaultProps: {
          disableElevation: true,
        },

        styleOverrides: {
          root: {
            borderRadius: 10,
            padding: "9px 16px",
          },
        },
      },

      MuiTextField: {
        defaultProps: {
          variant: "outlined",
        },
      },

      MuiOutlinedInput: {
        styleOverrides: {
          root: {
            borderRadius: 10,

            "& .MuiOutlinedInput-notchedOutline": {
              transition: "border-color 0.15s ease",
            },

            "&:hover .MuiOutlinedInput-notchedOutline": {
              borderColor: mode === "light" ? "#A1A1AA" : "#52525B",
            },

            "&.Mui-focused .MuiOutlinedInput-notchedOutline": {
              borderWidth: 2,
            },
          },
        },
      },

      MuiInputLabel: {
        styleOverrides: {
          root: {
            fontWeight: 500,
          },
        },
      },

      MuiCard: {
        styleOverrides: {
          root: {
            borderRadius: 14,
            border: `1px solid ${mode === "light" ? "#E4E4E7" : "#27272A"}`,
            boxShadow:
              mode === "light"
                ? "0 1px 3px rgba(0, 0, 0, 0.05)"
                : "0 1px 3px rgba(0, 0, 0, 0.3)",
          },
        },
      },

      MuiPaper: {
        styleOverrides: {
          root: {
            backgroundImage: "none",
          },
        },
      },

      MuiAppBar: {
        defaultProps: {
          elevation: 0,
        },

        styleOverrides: {
          root: {
            backgroundImage: "none",
            borderBottom: `1px solid ${
              mode === "light" ? "#E4E4E7" : "#27272A"
            }`,
          },
        },
      },

      MuiTooltip: {
        styleOverrides: {
          tooltip: {
            borderRadius: 8,
            fontSize: "0.75rem",
          },
        },
      },

      MuiChip: {
        styleOverrides: {
          root: {
            borderRadius: 8,
            fontWeight: 500,
          },
        },
      },
    },
  });
