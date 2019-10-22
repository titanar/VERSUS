import { Link, LinkGetProps, Router } from '@reach/router';
import React, { lazy, Suspense, useRef, useState } from 'react';
import { Icon, Loader, Menu } from 'semantic-ui-react';

import { experience } from '../../appSettings.json';
import { errors } from '../../terms.en-us.json';
import { deleteFrom } from '../../utilities/arrays';
import { promiseAfter } from '../../utilities/promises';
import { RoutedFC } from '../../utilities/routing';
import { routes } from '../routes';
import {
    IMessageContext,
    MessageContext,
    ShowErrorHandler,
    ShowInfoHandler,
    ShowInfoUntilHandler,
} from './header/MessageContext';
import { Snack } from './header/Snack';
import { ISnack, showSnack } from './header/snacks';

const Home = lazy(() => import('./Home').then(module => ({ default: module.Home })));
const Error = lazy(() => import('../shared/Error').then(module => ({ default: module.Error })));

export const Frontend: RoutedFC = () => {
  const { snackTimeout } = experience;

  const showSuccess: ShowInfoHandler = (text, timeout) => showInfo(text, timeout, 'success');

  const showInfo: ShowInfoHandler = (text, timeout, type = 'info') => {
    showSnack(
      text,
      type,
      snack => setSnacks(snacks => [...snacks, snack]),
      promiseAfter(timeout || snackTimeout)({}),
      snack => setSnacks(snacks => deleteFrom(snack, snacks))
    );
  };

  const showInfoUntil: ShowInfoUntilHandler = (text, isComplete, update?) => {
    showSnack(
      text,
      'update',
      snack => setSnacks(snacks => [...snacks, snack]),
      isComplete.then(promiseAfter(snackTimeout)),
      snack => setSnacks(snacks => deleteFrom(snack, snacks)),
      update
    );
  };

  const showWarning: ShowErrorHandler = (error, timeout) => {
    console.warn(error);

    const text = (error.body && error.body.message) || error.message;

    showInfo(text, timeout, 'warning');
  };

  const showError: ShowErrorHandler = (error, timeout) => {
    console.error(error);

    const text = (error.body && error.body.message) || error.message;

    showInfo(text, timeout, 'error');
  };

  const [snacks, setSnacks] = useState<ISnack[]>([]);

  const headerContext = useRef<IMessageContext>({
    showSuccess,
    showInfo,
    showInfoUntil,
    showWarning,
    showError
  });

  const setActiveWhenCurrent = (linkIsCurrent: (link: LinkGetProps) => boolean) => (link: LinkGetProps) => ({
    className: linkIsCurrent(link) ? 'active item' : 'item'
  });

  return (
    <MessageContext.Provider value={headerContext.current}>
      <div className='snack bar'>
        {snacks.map(snack => (
          <Snack {...snack} />
        ))}
      </div>
      <div className='app'>
        <div className='sidebar'>
          <Menu.Item as={Link} to={routes.home} getProps={setActiveWhenCurrent(link => link.isCurrent)}>
            <Icon name='home' />
          </Menu.Item>
        </div>
        <div className='main'>
          <Suspense fallback={<Loader active size='massive' />}>
            <Router>
              <Home path={routes.home} />
              <Error path={routes.error} default message={errors.notFound} />
            </Router>
          </Suspense>
        </div>
      </div>
    </MessageContext.Provider>
  );
};
