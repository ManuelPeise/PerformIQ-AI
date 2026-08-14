// pages/NotFoundPage.tsx

import { Box, Button, Typography } from "@mui/material";
import { useNavigate } from "react-router-dom";

const NotFoundPage: React.FC = () => {
  const navigate = useNavigate();

  return (
    <Box
      sx={{
        minHeight: "100%",
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "center",
        gap: 2,
      }}
    >
      <Typography variant="h1">404</Typography>

      <Typography variant="h5">Page not found</Typography>

      <Button variant="contained" onClick={() => navigate("/performiq-ai")}>
        Go Home
      </Button>
    </Box>
  );
};
export default NotFoundPage;
