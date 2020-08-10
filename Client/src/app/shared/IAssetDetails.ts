import { IAssetDescription } from './IAssetDescription';

export interface IAssetDetails {
  id: string;
  descriptions: IAssetDescription[];
  fileName: string;
  imageHeight: number | null;
  imageWidth: number | null;
  name: string;
  size: number;
  thumbnailUrl: string;
  title: string;
  type: string;
  url: string;
}
