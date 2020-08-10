import { IContentItem } from '../element/DeepClone';

export interface IDeepCloneResponse {
  totalApiCalls: number;
  totalMilliseconds: number;
  newItems: IContentItem[];
}
