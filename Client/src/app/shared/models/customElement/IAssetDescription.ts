interface ILanguage {
  id: string;
  codename: string;
}

export interface IAssetDescription {
  language: ILanguage;
  description: string;
}
