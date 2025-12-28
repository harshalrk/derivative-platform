import { Routes, Route, Link } from 'react-router-dom';
import { AppBar, Toolbar, Typography, Container, Box, Button } from '@mui/material';
import DefineCurves from './pages/DefineCurves';
import EnterQuotes from './pages/EnterQuotes';
import RollQuotes from './pages/RollQuotes';

function App() {
  return (
    <Box>
      <AppBar position="static">
        <Toolbar>
          <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
            Market Data Management
          </Typography>
          <Button color="inherit" component={Link} to="/define">
            Define Curves
          </Button>
          <Button color="inherit" component={Link} to="/quotes">
            Enter Quotes
          </Button>
          <Button color="inherit" component={Link} to="/roll">
            Roll Quotes
          </Button>
        </Toolbar>
      </AppBar>

      <Container maxWidth="lg" sx={{ mt: 4 }}>
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/define" element={<DefineCurves />} />
          <Route path="/quotes" element={<EnterQuotes />} />
          <Route path="/roll" element={<RollQuotes />} />
        </Routes>
      </Container>
    </Box>
  );
}

function HomePage() {
  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Welcome to Market Data Management
      </Typography>
      <Typography variant="body1" paragraph>
        This application manages market data curves and quotes for derivatives pricing.
      </Typography>
      <Typography variant="h6" gutterBottom>
        Features:
      </Typography>
      <ul>
        <li>Define market data curves with term structures</li>
        <li>Enter and manage daily quotes</li>
        <li>Roll curves and quotes to new dates</li>
      </ul>
    </Box>
  );
}

export default App;
