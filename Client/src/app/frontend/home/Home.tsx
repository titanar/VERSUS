import React, { useEffect, useRef, useState } from 'react';

import { toRounded } from '../../../utilities/numbers';
import { RoutedFC } from '../../../utilities/routing';
import { Field, IItem, IPlacedItem } from './Field';

export const Home: RoutedFC = () => {
  //   const dummyShirt: IShirt = {
  //     name: 'Dummy shirt ',
  //     codename: 'dummy_shirt_',
  //     description: 'Dummy description',
  //     images: [
  //       {
  //         name: 'Dummy image 1',
  //         description: 'image description',
  //         url: 'http://www.vshisher.com/Data/000248/000248-LA+Sweatsh1-hi.png',
  //         width: 750,
  //         height: 750
  //       }
  //     ],
  //     gender: 'Female',
  //     size: 15,
  //     colors: ['Blue'],
  //     price: 18.5,
  //     releaseDate: new Date('2019-10-22T14:27:51.7965118Z')
  //   };

  //   const dummyItems: IShirt[] = [];

  //   for (let index = 1; index < 25; index++) {
  //     const newShirt = { ...dummyShirt };

  //     newShirt.name += index;
  //     newShirt.codename += index;
  //     newShirt.size = toRounded(Math.random() * 15, 1);
  //     newShirt.price = toRounded(Math.random() * 35, 2);

  //     dummyItems.push(newShirt);
  //   }

  //   {dummyItems.map(item => (
  //     <Card>
  //       <Image src={item.images[0].url} wrapped ui={false} />
  //       <Card.Content>
  //         <Card.Header>{item.name}</Card.Header>
  //         <Card.Meta>
  //           ${item.price} {item.gender}
  //         </Card.Meta>
  //         <Card.Description>{item.description}</Card.Description>
  //       </Card.Content>
  //       <Card.Content extra>
  //         <Icon name='clock' />
  //         {item.releaseDate.toLocaleDateString()}
  //       </Card.Content>
  //     </Card>
  //   ))}

  const dummyItems: IItem[] = [
    {
      width: 2,
      height: 2
    },
    {
      width: 2,
      height: 3
    },
    {
      width: 3,
      height: 3
    },
    {
      width: 3,
      height: 4
    }
  ];

  const items: IItem[] = [];

  for (let index = 0; index < 100; index++) {
    const which = toRounded(Math.random() * (dummyItems.length - 1));

    items.push({ ...dummyItems[which] });
  }

  const [placedItems, setPlacedItems] = useState<IPlacedItem[]>([]);

  const homeRef = useRef<HTMLDivElement>(null);
  const sizerRef = useRef<HTMLDivElement>(null);
  const fieldRef = useRef<Field>();

  useEffect(() => {
    if (homeRef.current && sizerRef.current) {
      const { width: totalWidth, height: totalHeight } = homeRef.current.getBoundingClientRect();
      const sizerSize = sizerRef.current.getBoundingClientRect().width;

      const fieldWidth = (totalWidth - (totalWidth % sizerSize)) / sizerSize;
      const fieldHeight = (totalHeight - (totalHeight % sizerSize)) / sizerSize;

      fieldRef.current = new Field(fieldWidth, fieldHeight, items.shift());

      items.forEach(item => fieldRef.current && fieldRef.current.place(item));

      setPlacedItems(fieldRef.current.items);

      console.log(JSON.parse(JSON.stringify(fieldRef.current.field)));
    }
  }, []);

  return (
    <div ref={homeRef} className='home'>
      <div ref={sizerRef} className='sizer' />
      {placedItems.map((item, index) => (
        <div
          key={index}
          style={{
            width: `${item.width}0em`,
            height: `${item.height}0em`,
            left: `${item.cell.left}0em`,
            top: `${item.cell.top}0em`
          }}
          className={`item`}
        >
          <pre>
            {index}: {JSON.stringify(item, null, 4)}
          </pre>
        </div>
      ))}
    </div>
  );
};
