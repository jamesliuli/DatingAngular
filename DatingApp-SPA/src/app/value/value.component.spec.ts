import {compute} from './value.component';

describe('compute', () => {
    it('should return 0', () => {
        const result = compute(-1);
        expect(result).toBe(0);
    });

    it('should return increments', () => {
        const result = compute(1);
        expect(result).toBe(0);
    });

});

