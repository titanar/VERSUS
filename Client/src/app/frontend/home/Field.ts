import { toRounded } from '../../../utilities/numbers';

export interface IItem {
  width: number;
  height: number;
}

export interface IPlacedItem extends IItem {
  cell: ICell;
}

interface ICell {
  top: number;
  left: number;
}

interface ICandidateCell {
  cell: ICell;
  overlappingCells: number;
}

export class Field {
  width: number;
  height: number;
  field: number[][] = [];
  items: IPlacedItem[] = [];

  constructor(width: number, height: number, seedItem?: IItem) {
    this.width = width;
    this.height = height;
    this.field = new Array<number[]>(height);

    for (var i = 0; i < height; i++) {
      this.field[i] = new Array<number>(width).fill(0);
    }

    if (seedItem) {
      this.placeAtCell(seedItem, {
        top: toRounded(height / 2) - 1,
        left: toRounded(width / 2)
      });
    }
  }

  addRow() {
    this.field.push(new Array<number>(this.width).fill(0));
    this.height++;
  }

  fillCell(cell: ICell) {
    this.field[cell.top][cell.left] = 1;
  }

  getBorderCells() {
    const borderCells: ICell[] = [];

    for (let top = 0; top < this.height; top++) {
      for (let left = 0; left < this.width; left++) {
        if (this.field[top][left] === 1) {
          const immediateBorderCells: ICell[] = [
            { top: top - 1, left: left - 1 },
            { top: top - 1, left: left },
            { top: top - 1, left: left + 1 },
            { top: top, left: left + 1 },
            { top: top + 1, left: left + 1 },
            { top: top + 1, left: left },
            { top: top + 1, left: left - 1 },
            { top: top, left: left - 1 }
          ];

          immediateBorderCells.forEach(cell => {
            if (
              !borderCells.some(borderCell => borderCell.left === cell.left && borderCell.top === cell.top) &&
              this.field[cell.top] !== undefined &&
              this.field[cell.top][cell.left] === 0
            ) {
              borderCells.push(cell);
            }
          });
        }
      }
    }

    return borderCells;
  }

  getOverlappingCells(item: IItem, cell: ICell, sourceCells: ICell[]) {
    let overlappingCells = 0;

    for (let top = cell.top; top < item.height + cell.top; top++) {
      for (let left = cell.left; left < item.width + cell.left; left++) {
        if (sourceCells.some(cell => cell.top === top && cell.left === left)) {
          overlappingCells++;
        }
      }
    }
    return overlappingCells;
  }

  itemCanBePlaced(item: IItem, cell: ICell) {
    for (let top = cell.top; top < item.height + cell.top; top++) {
      for (let left = cell.left; left < item.width + cell.left; left++) {
        if (this.field[top] === undefined || this.field[top][left] === undefined || this.field[top][left] === 1) {
          return false;
        }
      }
    }

    return true;
  }

  place(item: IItem) {
    let placed = false;

    while (!placed) {
      const borderCells = this.getBorderCells();

      const candidateCells: ICandidateCell[] = [];

      borderCells.forEach(cell => {
        for (let top = cell.top; top > cell.top - item.height; top--) {
          for (let left = cell.left; left > cell.left - item.width; left--) {
            if (this.itemCanBePlaced(item, { top, left })) {
              let overlappingCells = this.getOverlappingCells(item, { top, left }, borderCells);

              candidateCells.push({ cell: { top, left }, overlappingCells });
            }
          }
        }
      });

      candidateCells.sort((a, b) => b.overlappingCells - a.overlappingCells);

      if (candidateCells[0]) {
        this.placeAtCell(item, candidateCells[0].cell);
        placed = true;
      } else {
        this.addRow();
      }
    }
  }

  placeAtCell(item: IItem, cell: ICell) {
    for (let top = cell.top; top < item.height + cell.top; top++) {
      for (let left = cell.left; left < item.width + cell.left; left++) {
        this.fillCell({ top, left });
      }
    }

    this.items.push({ ...item, cell });
  }
}
