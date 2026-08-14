import type { ICurrentUser } from "../../../lib/types/auth/ICurrentUser";
import type { ILoginRequestModel } from "../../../lib/types/auth/ILoginRequestModel";
import type { IRegisterRequestModel } from "../../../lib/types/auth/IRegisterRequestModel";

export interface IAuthenticationContext {
  currentUser: ICurrentUser | null;
  onAuthenticateUser: (loginRequest: ILoginRequestModel) => Promise<boolean>;
  onRegisterUser: (registerRequest: IRegisterRequestModel) => Promise<boolean>;
  onLogoutUser: () => Promise<void>;
}
