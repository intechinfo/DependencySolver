using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Invenietis.DependencySolver.Core
{
    sealed class ItemContainer<TId, TData, TItem>
    {
        readonly Func<TId, TData, TItem> _itemFactory;
        readonly Func<TItem, TId> _idSelector;
        readonly Dictionary<TId, TItem> _items;

        internal ItemContainer(
            Func<TId, TData, TItem> itemFactory,
            Func<TItem, TId> idSelector )
        {
            Debug.Assert( itemFactory != null );
            _itemFactory = itemFactory;
            _idSelector = idSelector;
            _items = new Dictionary<TId, TItem>();
        }

        internal ItemContainer( Func<TId, TData, TItem> itemFactory )
            : this( itemFactory, null )
        {
        }

        internal TItem CreateItem( TId id, TData data )
        {
            var item = _itemFactory( id, data );
            _items.Add( id, item );
            return item;
        }

        internal void AddItem( TItem item )
        {
            Debug.Assert( _idSelector != null );
            _items.Add( _idSelector( item ), item );
        }

        internal IReadOnlyList<TItem> Items => _items.Values.ToList();
    }
}
