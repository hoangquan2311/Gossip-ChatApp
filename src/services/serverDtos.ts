export type MemberDto = {
  userId: string;
  displayName: string;
  email: string;
  avatarUrl?: string;
};

export type GroupDto = {
  id: string;
  title: string;
  avatarUrl?: string;
  members: MemberDto[];
};