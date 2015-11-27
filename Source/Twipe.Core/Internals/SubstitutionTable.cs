using System;
using System.Collections.Generic;

namespace Twipe.Core.Internals
{
    public class SubstitutionTable<T> : ISubstitutionTable<T>
    {
        private IList<SubstitutionItem<T>>[] grayShades;
        private ShadeSubstitutionAcceptance acceptanceRange;
        private Random rand;
        private const int MaxShadeValue = 255;
        private readonly double QuarterOfAShadeDifference = 0.25 / MaxShadeValue;

        // this constructor is for unit testing purposes only
        internal SubstitutionTable(IList<SubstitutionItem<T>>[] grayShadesArray)
        {
            grayShades = grayShadesArray;
        }

        public SubstitutionTable()
        {
            grayShades = new List<SubstitutionItem<T>>[MaxShadeValue + 1];

            for (int i = 0; i <= MaxShadeValue; i++)
                grayShades[i] = new List<SubstitutionItem<T>>();

            acceptanceRange = ShadeSubstitutionAcceptance.HalfAShade;
            //acceptanceRange = ShadeSubstitutionAcceptance.QuarterOfAShade;

            rand = new Random();
        }

        public ShadeSubstitutionAcceptance AcceptanceRange
        {
            get { return acceptanceRange; }

            set { acceptanceRange = value; }
        }

        public void SetSubstitution(double grayRatio, T item)
        {
            SetSubstitution(new SubstitutionItem<T>(grayRatio, item));
        }

        public void SetSubstitution(SubstitutionItem<T> substitutionItem)
        {
            int index = (int)Math.Round(substitutionItem.GrayRatio * MaxShadeValue,
                                        0, MidpointRounding.AwayFromZero);

            double difference = Math.Abs((index / MaxShadeValue) - substitutionItem.GrayRatio);

            switch (acceptanceRange)
            {
                case ShadeSubstitutionAcceptance.Zero:
                    if (difference < 0)
                        grayShades[index].Add(substitutionItem);
                    break;

                case ShadeSubstitutionAcceptance.QuarterOfAShade:
                    if (difference < QuarterOfAShadeDifference)
                        grayShades[index].Add(substitutionItem);
                    break;

                case ShadeSubstitutionAcceptance.HalfAShade:
                    grayShades[index].Add(substitutionItem);
                    break;
            }
        }

        public T GetSubstitutionFor(int grayShadeValue)
        {
            int substitutionCount = grayShades[grayShadeValue].Count;

            if (substitutionCount == 0)
                return GetClosestSubstitution(grayShadeValue);
            else if (substitutionCount > 1)
                return PickRandomly(grayShades[grayShadeValue]);
            else
                return grayShades[grayShadeValue][0].Value;
        }

        public void Dispose()
        {
            IDisposable garbage;

            foreach (List<SubstitutionItem<T>> list in grayShades)
            {
                foreach (SubstitutionItem<T> item in list)
                {
                    garbage = item.Value as IDisposable;

                    if (garbage != null)
                        garbage.Dispose();
                }
            }
        }

        private T PickRandomly(IList<SubstitutionItem<T>> list)
        {
            int index = rand.Next(0, list.Count - 1);

            return list[index].Value;
        }

        private T GetClosestSubstitution(int grayShadeValue)
        {
            int offset = 0;
            bool shouldAdd = true;
            int offsetLimit;
            int currentIndex = grayShadeValue;

            int upperBound = (grayShades.Length - 1) - grayShadeValue;
            int lowerBound = grayShadeValue - 0;

            if (upperBound > lowerBound)
                offsetLimit = lowerBound;
            else
                offsetLimit = upperBound;

            for (; offset <= (2 * offsetLimit); offset++, shouldAdd = !shouldAdd)
            {
                if (shouldAdd)
                    currentIndex += offset;
                else
                    currentIndex -= offset;

                if (grayShades[currentIndex].Count >= 1)
                    return PickRandomly(grayShades[currentIndex]);
            }

            if (upperBound > lowerBound)
            {
                for (; offset <= upperBound; offset++)
                    if (grayShades[grayShadeValue + offset].Count >= 1)
                        return PickRandomly(grayShades[grayShadeValue + offset]);
            }
            else
            {
                for (; offset >= lowerBound; offset++)
                    if (grayShades[grayShadeValue - offset].Count >= 1)
                        return PickRandomly(grayShades[grayShadeValue - offset]);
            }

            return default(T);
        }
    }
}