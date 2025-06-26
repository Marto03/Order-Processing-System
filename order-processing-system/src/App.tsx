import './App.css';
import { OrderList } from './components/OrderList';

function App() {
    return (
        <div className="App" style={{ maxWidth: 800, margin: '0 auto', padding: 20 }}>
            <h1>Панел за поръчки</h1>
            <OrderList />
        </div>
    );
}

export default App;
