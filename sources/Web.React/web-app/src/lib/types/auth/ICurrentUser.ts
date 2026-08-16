export interface ICurrentUser {
  userId: number;
  userName: string;
  email: string;
  roles: string[];
  permissions: IPermission[];
}

export interface IPermission {
  module: string;
  groupResourceKey: string;
  canView: boolean;
  canCreate: boolean;
  canEdit: boolean;
  canDelete: boolean;
}
