import React from "react";
import type { ILocalizationProps } from "../../lib/localization/withLocalization";
import { withLocalization } from "../../lib/localization/withLocalization";
import StyledBox from "../../components/styledComponents/boxes";
import StyledCard from "../../components/styledComponents/cards";
import StyledCaption from "../../components/styledComponents/typography";
import StyledDivider from "../../components/styledComponents/dividers";
import {
  ConfirmButton,
  CancelButton,
} from "../../components/styledComponents/buttons";
import { useNavigate } from "react-router-dom";
import FormTextField from "../../components/form/FormTextField";
import type { ILoginRequestModel } from "../../lib/types/auth/ILoginRequestModel";
import { useForm } from "../../hooks/useForm";
import FormLink from "../../components/form/FormLink";
import {
  validateEmailAddress,
  validateStringLength,
} from "../../lib/validation";
import { useAuthenticationContext } from "../../hooks/useAuthenticationContext";

const validationCallback = (state: ILoginRequestModel): boolean => {
  return (
    validateEmailAddress(state.userNameOrEmail) &&
    validateStringLength(state.password, 8)
  );
};

const initializationModel: ILoginRequestModel = {
  userNameOrEmail: "testadmin@gmail.com",
  password: "P@ssword-123",
};

interface IProps extends ILocalizationProps {}

const LoginPage: React.FC<IProps> = (props) => {
  const { getResource } = props;
  const navigation = useNavigate();

  const { onAuthenticateUser } = useAuthenticationContext();
  const loginForm = useForm<ILoginRequestModel>(
    initializationModel,
    validationCallback,
  );

  const { userNameOrEmail, password } = loginForm.subscribe((state) => ({
    userNameOrEmail: state.userNameOrEmail,
    password: state.password,
  }));

  const handleLoginClick = React.useCallback(async () => {
    const authState = await onAuthenticateUser(loginForm.state);

    if (authState === true) {
      navigation("/performiq-ai");
    }
  }, [onAuthenticateUser, loginForm]);

  const handleCancelClick = React.useCallback(() => {
    loginForm.resetForm();
    navigation("/");
  }, [loginForm, navigation]);

  return (
    <StyledBox
      sx={{
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        padding: 2,
      }}
    >
      <StyledCard sx={{ margin: 0, padding: 2, opacity: 0.8, minWidth: 400 }}>
        <StyledBox
          sx={{
            flexDirection: "column",
            justifyContent: "center",
            alignItems: "center",
          }}
        >
          <StyledCaption variant="h1">
            {getResource("common.labelLogin")}
          </StyledCaption>
          <StyledDivider variant="inset" />
          <FormTextField
            label={getResource("common.labelUsername")}
            value={userNameOrEmail}
            onChange={(value) =>
              loginForm.handleChange({
                currentModel: { ...loginForm.state, userNameOrEmail: value },
              })
            }
          />
          <FormTextField
            label={getResource("common.labelPassword")}
            value={password}
            onChange={(value) =>
              loginForm.handleChange({
                currentModel: { ...loginForm.state, password: value },
              })
            }
            type="password"
          />
          <StyledDivider variant="middle" />
          <FormLink
            href="/auth/register"
            linkText={getResource("common.labelDontHaveAccount")}
            text={getResource("common.labelCreateAccount")}
          />
        </StyledBox>
        <StyledBox
          sx={{
            display: "flex",
            justifyContent: "flex-end",
            alignItems: "center",
          }}
        >
          <CancelButton onClick={handleCancelClick}>
            {getResource("common.labelCancel")}
          </CancelButton>
          <ConfirmButton
            onClick={handleLoginClick}
            disabled={!loginForm.isValid}
          >
            {getResource("common.labelAuthorize")}
          </ConfirmButton>
        </StyledBox>
      </StyledCard>
    </StyledBox>
  );
};

export default withLocalization(["common"], LoginPage);
