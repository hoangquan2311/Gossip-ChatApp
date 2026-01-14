export interface IUser {
  userId: string;
  displayName: string;
  email: string;
  avatarUrl?: string;
}

export interface IMessage {
  id: string;
  type?: string;
  content: string;
  sentAt: string;
  sender: IUser;
}

export interface IConversation {
  groupId: string;
  title?: string;
  avatarUrl?: string;
  messages: IMessage[];
  // unread: number;
  members: IUser[];
}

export interface RegisterPersonalForm {
  email: string;
  firstName: string;
  lastName: string;
}

export interface RegisterPasswordForm {
  password: string;
  confirmPassword: string;
}

export interface LoginForm {
  email: string;
  password: string;
}