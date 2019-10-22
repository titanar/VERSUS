export interface IShirt {
  name: string;
  codename: string;
  description: string;
  images: IImage[];
  gender: string;
  size: number;
  colors: string[];
  price: number;
  releaseDate: Date;
}

export interface IImage {
  name: string;
  description: string;
  url: string;
  width: number;
  height: number;
}
