import { BehaviorSubject } from 'rxjs';

import { IMessageContext } from '../../app/frontend/header/MessageContext';
import { IShirt } from './kenticoKontent';

export const IKenticoKontentService = 'IKenticoKontentService';

export interface IKenticoKontentService {
  shirts: BehaviorSubject<IShirt[] | undefined>;
  messageContext: IMessageContext;
}

export class KenticoKontentService implements IKenticoKontentService {
  shirts: BehaviorSubject<IShirt[] | undefined> = new BehaviorSubject<IShirt[] | undefined>(undefined);
  messageContext!: IMessageContext;

  async getShirts() {
    const { showError } = this.messageContext;

    let shirts!: IShirt[];

    try {
      //  transfers = response.data.transfers;
    } catch (error) {
      showError(error);
    }

    this.shirts.next(shirts);
  }
}
