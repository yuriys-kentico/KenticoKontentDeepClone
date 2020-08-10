interface IItem {
  id: string;
  codename: string;
}

interface IVariant {
  id: string;
  codename: string;
}

export interface IContext {
  projectId: string;
  item: IItem;
  variant: IVariant;
}
