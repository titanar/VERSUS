import { navigate, RouteComponentProps } from '@reach/router';
import Axios from 'axios';
import { FC } from 'react';

import { routes } from '../app/routes';
import { errors, home } from '../terms.en-us.json';

Axios.interceptors.response.use(undefined, error => {
  if (error.response && error.response.status === 404) {
    navigate(routes.error, {
      state: {
        message: errors.genericError,
        stack: error.stack
      }
    });
  }
});

export type RoutedFC<P = {}> = FC<RouteComponentProps<P>>;

export const setTitle = (section?: string) => {
  let title = home.header;

  if (section) {
    title += ` | ${section}`;
  }

  document.title = title;
};
