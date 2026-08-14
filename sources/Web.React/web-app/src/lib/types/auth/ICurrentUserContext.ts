import type { ICurrentUser } from "./ICurrentUser";

export interface ICurrentUserContext {
  currentUser: ICurrentUser | null;
  jwtToken?: string | null;
  assignCurrentUser: () => Promise<void>;
  updateJwtToken: (token: string) => void;
  unassignCurrentUser: () => void;
}
