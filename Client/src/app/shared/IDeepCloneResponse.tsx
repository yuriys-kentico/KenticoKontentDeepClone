import { IContentItem } from './models/management/IContentItem';

export interface IDeepCloneResponse {
  totalApiCalls: number;
  totalMilliseconds: number;
  newItems: IContentItem[];
}
