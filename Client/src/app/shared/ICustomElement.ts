import { IAssetDetails } from './IAssetDetails';
import { IContext } from './IContext';

interface IAssetReference {
  id: string;
}

interface ISelectAssetConfig {
  allowMultiple: boolean;
  fileType: 'all' | 'images';
}

export interface ICustomElement<TConfig> {
  value: string | null;
  disabled: boolean;
  config: TConfig | null;
  init: (callback: (element: ICustomElement<TConfig>, context: IContext) => void) => void;
  setValue: (value: string) => void;
  setHeight: (value: number) => void;
  onDisabledChanged: (callback: (disabled: boolean) => void) => void;
  selectAssets: (config: ISelectAssetConfig) => Promise<IAssetReference[]>;
  getAssetDetails: (assetIds: string[]) => Promise<IAssetDetails[]>;
}
