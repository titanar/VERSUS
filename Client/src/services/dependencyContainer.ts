import { rootInjector } from 'typed-inject';

import { IKenticoKontentService, KenticoKontentService } from './kenticoKontent/kenticoKontentService';

const dependencies = rootInjector.provideClass(IKenticoKontentService, KenticoKontentService);

export const useDependency = dependencies.resolve.bind(dependencies);
