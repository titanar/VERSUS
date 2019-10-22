import React, { useEffect, useRef, useState } from 'react';

import { toRounded } from '../../utilities/numbers';
import { RoutedFC } from '../../utilities/routing';

interface IItemData {
  width: number;
  height: number;
}
interface IItem extends IItemData {
  origin: IOrigin;
}

interface IOrigin {
  left: number;
  top: number;
  quadrant: number;
  orientation: '+' | '-';
  width: number;
  height: number;
}

interface ILine {
  left: number;
  top: number;
  length: number;
}

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

  const items: IItemData[] = [
    {
      width: 2,
      height: 2
    },
    {
      width: 2,
      height: 3
    }
  ];

  const [placedItems, setPlacedItems] = useState<IItem[]>([]);

  const homeRef = useRef<HTMLDivElement>(null);
  const sizerRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (homeRef.current && sizerRef.current) {
      const { width: totalWidth, height: totalHeight } = homeRef.current.getBoundingClientRect();
      const sizerSize = sizerRef.current.getBoundingClientRect().width;

      const fieldWidth = (totalWidth - (totalWidth % sizerSize)) / sizerSize;
      const fieldHeight = (totalHeight - (totalHeight % sizerSize)) / sizerSize;

      let origins: IOrigin[] = [
        {
          left: toRounded(fieldWidth / 2),
          top: toRounded(fieldHeight / 2),
          quadrant: 2,
          orientation: '+',
          width: 4,
          height: 4
        }
      ];

      const newItems: IItem[] = [];

      for (const item of items) {
        const nextOrigin = origins.shift();

        if (nextOrigin) {
          switch (nextOrigin.orientation) {
            case '-':
              const oldHeight = item.height;
              item.height = item.width;
              item.width = oldHeight;
              break;
          }

          switch (nextOrigin.quadrant) {
            case 1:
              nextOrigin.top = nextOrigin.top - item.height;
              break;
            case 3:
              nextOrigin.left = nextOrigin.left - item.width;
              break;
            case 4:
              nextOrigin.top = nextOrigin.top - item.height;
              nextOrigin.left = nextOrigin.left - item.width;
              break;

            default:
              break;
          }

          const newItem: IItem = {
            width: item.width,
            height: item.height,
            origin: nextOrigin
          };

          newItems.push(newItem);

          origins = calculateNewOrigins(fieldWidth, newItems);
        }
      }

      setPlacedItems(newItems);
      console.log(origins);
    }
  }, []);

  const calculateNewOrigins = (fieldWidth: number, items: IItem[]) => {
    const origins: IOrigin[] = [];

    const negatives: ILine[] = items
      .map(item => ({
        left: item.origin.left,
        top: item.origin.top,
        length: item.width
      }))
      .concat(
        items.map(item => ({
          left: item.origin.left,
          top: item.origin.top + item.height,
          length: item.width
        }))
      )
      .sort((a, b) => (a.top > b.top ? 1 : -1));

    const positives: ILine[] = items
      .map(item => ({
        left: item.origin.left,
        top: item.origin.top,
        length: item.height
      }))
      .concat(
        items.map(item => ({
          left: item.origin.left + item.width,
          top: item.origin.top,
          length: item.height
        }))
      )
      .sort((a, b) => (a.left > b.left ? 1 : -1));

    for (const item of items) {
      const { width, height, origin } = item;

      const positiveAbove = positives.filter(positive => positive.left >= origin.left && positive.top < origin.top)[0];
      const originWidth = positiveAbove ? positiveAbove.left - origin.left : 4;

      const negativeBelow = negatives.filter(negative => negative.left <= origin.left && negative.top < origin.top)[0];
      const originHeight = negativeBelow ? negativeBelow.top - origin.top : 4;

      if (originWidth > 0 && originHeight > 0) {
        origins.push({
          left: origin.left,
          top: origin.top,
          orientation: '-',
          quadrant: 1,
          width: originWidth,
          height: originHeight
        });
      }

      const positiveBelow = positives.filter(
        positive => positive.left <= origin.left + width && positive.top < origin.top
      )[0];
      const originWidth2 = positiveBelow ? origin.left + width - positiveAbove.left : 4;

      if (originWidth2 > 0 && originHeight > 0) {
        origins.push({
          left: origin.left + width,
          top: origin.top,
          orientation: '-',
          quadrant: 4,
          width: originWidth2,
          height: originHeight
        });
      }

      const positiveAbove2 = positives.filter(
        positive => positive.left > origin.left + width && positive.top >= origin.top
      )[0];
      const originWidth3 = positiveAbove2 ? positiveAbove2.left - origin.left + width : 4;

      const negativeAbove2 = negatives.filter(
        negative => negative.left + negative.length > origin.left + origin.width && negative.top >= origin.top
      )[0];
      const originHeight2 = negativeAbove2 ? negativeAbove2.top - origin.top : 4;

      if (originWidth3 > 0 && originHeight2 > 0) {
        origins.push({
          left: origin.left + width,
          top: origin.top,
          orientation: '+',
          quadrant: 2,
          width: originWidth3,
          height: originHeight2
        });
      }

      //   const negativeBelow = negatives.filter(negative => negative.top <= origin.top + height)[1];
      //   const originHeight3 = negativeBelow ? negativeBelow.top - origin.top + height : 4;

      //   if (!(originWidth3 === 0 && originHeight3 === 0)) {
      //     origins.push({
      //       left: origin.left + width,
      //       top: origin.top + height,
      //       orientation: '+',
      //       quadrant: 1,
      //       width: originWidth3,
      //       height: originHeight3
      //     });
      //   }

      // origins.push({
      //   left: origin.left + width,
      //   top: origin.top + height,
      //   orientation: '-',
      //   quadrant: 3
      // });
      // origins.push({
      //   left: origin.left,
      //   top: origin.top + height,
      //   orientation: '-',
      //   quadrant: 2
      // });
      // origins.push({
      //   left: origin.left,
      //   top: origin.top + height,
      //   orientation: '+',
      //   quadrant: 4
      // });
      // origins.push({
      //   left: origin.left,
      //   top: origin.top,
      //   orientation: '+',
      //   quadrant: 3
      // });
    }

    return origins;
  };

  return (
    <div ref={homeRef} className='home'>
      <div ref={sizerRef} className='sizer' />
      {placedItems.map((item, index) => (
        <div
          key={index}
          style={{
            width: `${item.width}0em`,
            height: `${item.height}0em`,
            left: `${item.origin.left}0em`,
            top: `${item.origin.top}0em`
          }}
          className={`item`}
        >
          <pre>{JSON.stringify(item, null, 4)}</pre>
        </div>
      ))}
    </div>
  );
};
